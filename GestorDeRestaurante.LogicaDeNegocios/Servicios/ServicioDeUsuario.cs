using GestorDeRestaurante.AccesoADatos.Contexto;
using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios;

public class ServicioDeUsuario : IServicioDeUsuario
{
    private readonly DbContexto _contexto;
    private readonly IServicioDeEmail _servicioDeEmail;

    public ServicioDeUsuario(DbContexto contexto, IServicioDeEmail servicioDeEmail)
    {
        _contexto = contexto;
        _servicioDeEmail = servicioDeEmail;
    }

    public async Task<IEnumerable<ListarUsuarioDto>> ListarUsuariosAsync(string? filtroNombre)
    {
        var consulta = _contexto.Usuarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroNombre))
        {
            consulta = consulta.Where(u => u.Nombre.Contains(filtroNombre));
        }

        return await consulta.Select(u => new ListarUsuarioDto(
            u.Id,
            u.Identificacion,
            u.Nombre,
            u.Apellidos,
            u.Email,
            u.Rol.ToString()
        )).ToListAsync();
    }

    public async Task<(bool Exitoso, string Mensaje)> CrearUsuarioAsync(CrearUsuarioDto dto)
    {
        if (await _contexto.Usuarios.AnyAsync(u => u.Identificacion == dto.Identificacion))
            return (false, "La identificación ya está registrada.");

        if (await _contexto.Usuarios.AnyAsync(u => u.Email == dto.Email))
            return (false, "El correo electrónico ya está registrado.");

        if (await _contexto.Usuarios.AnyAsync(u => u.NombreUsuario == dto.NombreUsuario))
            return (false, "El nombre de usuario ya existe.");

        string contrasenaTemporal = Guid.NewGuid().ToString("N").Substring(0, 8);

        var nuevoUsuario = new Usuario
        {
            Identificacion = dto.Identificacion.Trim(),
            Nombre = dto.Nombre.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Email = dto.Email.Trim(),
            NombreUsuario = dto.NombreUsuario.Trim(),
            Contrasena = contrasenaTemporal,
            Rol = Enum.Parse<Rol>(dto.Rol)
        };

        _contexto.Usuarios.Add(nuevoUsuario);
        await _contexto.SaveChangesAsync();

        await _servicioDeEmail.EnviarEmail(
            nuevoUsuario.Email,
            $"CREACIÓN DEL USUARIO {nuevoUsuario.Nombre}",
            $"Bienvenido. La contraseña temporal de su usuario es: {contrasenaTemporal}");

        return (true, "Usuario creado exitosamente.");
    }

    public async Task<(bool Exitoso, string Mensaje)> EditarUsuarioAsync(int id, EditarUsuarioDto dto)
    {
        var usuario = await _contexto.Usuarios.FindAsync(id);
        if (usuario == null) return (false, "Usuario no encontrado.");

        if (dto.Email != usuario.Email && await _contexto.Usuarios.AnyAsync(u => u.Email == dto.Email))
            return (false, "El nuevo correo electrónico ya está registrado.");

        usuario.Nombre = dto.Nombre.Trim();
        usuario.Apellidos = dto.Apellidos.Trim();
        usuario.Email = dto.Email.Trim();
        usuario.Rol = Enum.Parse<Rol>(dto.Rol);

        await _contexto.SaveChangesAsync();
        return (true, "Usuario actualizado correctamente.");
    }

    public async Task<EditarUsuarioDto?> ObtenerUsuarioAEditarAsync(int id)
    {
        var usuario = await _contexto.Usuarios.FindAsync(id);

        if (usuario == null) return null;

        return new EditarUsuarioDto
        {
            Identificacion = usuario.Identificacion,
            Nombre = usuario.Nombre,
            Apellidos = usuario.Apellidos,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString()
        };
    }
}