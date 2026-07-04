using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces;

public interface IServicioDeAutenticacion
{
    Task<(bool Exitoso, string Mensaje, Rol? Rol)> ValidarCredencialesDeInicioDeSesionAsync(LoginDto request);
    Task<(bool Exitoso, string Mensaje)> CambiarContrasenaAsync(CambiarContrasenaDto request);
}