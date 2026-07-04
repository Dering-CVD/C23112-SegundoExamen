using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.Mobile.Servicios.Interfaces;
using System.Text.Json;

namespace GestorDeRestaurante.Mobile.Servicios.Implementaciones;

public class ServicioMenu : IServicioMenu
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ServicioMenu()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(Constantes.BaseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", Constantes.ApiKey);
    }

    public async Task<List<Platillo>> ObtenerPlatillosAsync()
    {
        var response = await _httpClient.GetAsync("api/Menu/ObtengaLaListaDePlatillos");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Platillo>>(content, _jsonOptions) ?? [];
    }

    public async Task<List<Platillo>> BuscarPlatillosPorNombreAsync(string nombre)
    {
        var response = await _httpClient.GetAsync($"api/Menu/ObtengaLaListaDePlatillosPorNombre?nombre={Uri.EscapeDataString(nombre)}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Platillo>>(content, _jsonOptions) ?? [];
    }

    public async Task<Platillo?> ObtenerPlatilloPorIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/Menu/ObtengaElPlatilloPorId?id={id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Platillo>(content, _jsonOptions);
    }
}
