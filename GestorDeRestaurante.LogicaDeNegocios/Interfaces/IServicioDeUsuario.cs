using GestorDeRestaurante.Dominio.Dtos;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces;

public interface IServicioDeUsuario
{
    Task<IEnumerable<ListarUsuarioDto>> ListarUsuariosAsync(string? filtroNombre);
    Task<(bool Exitoso, string Mensaje)> CrearUsuarioAsync(CrearUsuarioDto dto);
    Task<EditarUsuarioDto?> ObtenerUsuarioAEditarAsync(int id);
    Task<(bool Exitoso, string Mensaje)> EditarUsuarioAsync(int id, EditarUsuarioDto dto);
}