using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.Mobile.Servicios.Interfaces;

namespace GestorDeRestaurante.Mobile.Views;

public partial class ListaMenu : ContentPage
{
    private readonly IServicioMenu _servicioDeMenu;
    private CancellationTokenSource? _searchCts;

    public ListaMenu()
    {
        InitializeComponent();
        _servicioDeMenu = IPlatformApplication.Current!.Services!.GetRequiredService<IServicioMenu>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarPlatillosAsync();
    }

    private async void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(400, _searchCts.Token);
            await CargarPlatillosAsync();
        }
        catch (TaskCanceledException) { }
    }

    private async void OnPlatilloSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Platillo platillo)
        {
            await Navigation.PushAsync(new DetalleMenu(platillo));
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async Task CargarPlatillosAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            List<Platillo> platillos;

            if (string.IsNullOrWhiteSpace(SearchBar.Text))
                platillos = await _servicioDeMenu.ObtenerPlatillosAsync();
            else
                platillos = await _servicioDeMenu.BuscarPlatillosPorNombreAsync(SearchBar.Text);

            MenuCollectionView.ItemsSource = platillos;

            if (platillos.Count == 0)
                EmptyMessage.Text = string.IsNullOrWhiteSpace(SearchBar.Text)
                    ? "No hay platillos disponibles en este momento."
                    : "No se encontraron platillos con ese nombre.";
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue &&
            (ex.StatusCode.Value == System.Net.HttpStatusCode.Unauthorized ||
             ex.StatusCode.Value == System.Net.HttpStatusCode.Forbidden))
        {
            await DisplayAlertAsync("Acceso denegado", "API Key inválida.", "OK");
        }
        catch (HttpRequestException)
        {
            await DisplayAlertAsync("Error de conexión", "No se puede conectar con el servidor.", "OK");
        }
        catch (TaskCanceledException)
        {
            await DisplayAlertAsync("Tiempo de espera", "El servidor no respondió a tiempo.", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}