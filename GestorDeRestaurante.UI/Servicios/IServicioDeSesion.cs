using GestorDeRestaurante.Dominio.Entidades;

namespace GestorDeRestaurante.UI.Servicios;

public interface IServicioDeSesion
{
    Task GenerarSesionAsync(string nombreUsuario, Rol rol);
    Task FinalizarSesionAsync();
}