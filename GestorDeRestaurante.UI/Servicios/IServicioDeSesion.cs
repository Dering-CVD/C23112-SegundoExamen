using SistemaAlquilerPlaya.Dominio.Entidades;

namespace SistemaAlquilerPlaya.UI.Servicios;

public interface IServicioDeSesion
{
    Task GenerarSesionAsync(string nombreUsuario, Rol rol);
    Task FinalizarSesionAsync();
}