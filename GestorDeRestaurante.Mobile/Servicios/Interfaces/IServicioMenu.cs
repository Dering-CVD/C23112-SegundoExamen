using GestorDeRestaurante.Dominio.Entidades;

namespace GestorDeRestaurante.Mobile.Servicios.Interfaces;

public interface IServicioMenu
{
    Task<List<Platillo>> ObtenerPlatillosAsync();
    Task<List<Platillo>> BuscarPlatillosPorNombreAsync(string nombre);
    Task<Platillo?> ObtenerPlatilloPorIdAsync(int id);
}
