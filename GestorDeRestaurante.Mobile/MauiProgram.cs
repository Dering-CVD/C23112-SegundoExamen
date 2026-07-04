using GestorDeRestaurante.Mobile.Servicios.Implementaciones;
using GestorDeRestaurante.Mobile.Servicios.Interfaces;
using GestorDeRestaurante.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace GestorDeRestaurante.Mobile
{
public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IServicioMenu, ServicioMenu>();
            builder.Services.AddTransient<ListaMenu>();
            builder.Services.AddTransient<DetalleMenu>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
