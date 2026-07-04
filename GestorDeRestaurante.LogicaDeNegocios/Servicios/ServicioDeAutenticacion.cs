using GestorDeRestaurante.AccesoADatos.Contexto;
using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios;

public class ServicioDeAutenticacion : IServicioDeAutenticacion
{
    private readonly DbContexto _contexto;
    private readonly IServicioDeEmail _servicioDeEmail;

    public ServicioDeAutenticacion(DbContexto contexto, IServicioDeEmail servicioDeEmail)
    {
        _contexto = contexto;
        _servicioDeEmail = servicioDeEmail;
    }

    public async Task<(bool Exitoso, string Mensaje, Rol? Rol)> ValidarCredencialesDeInicioDeSesionAsync(LoginDto request)
    {
        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario);

        if (usuario == null) return (false, "Credenciales inválidas", null);

        if (usuario.Rol != Rol.Administrador && usuario.BloqueadoHasta.HasValue)
        {
            if (usuario.BloqueadoHasta <= DateTime.Now)
            {
                usuario.IntentosFallidos = 0;
                usuario.BloqueadoHasta = null;
                await _contexto.SaveChangesAsync();
            }
            else
            {
                return (false, $"Cuenta bloqueada. Intente más tarde a las {usuario.BloqueadoHasta.Value:hh:mm}", null);
            }
        }

        if (usuario.Contrasena != request.Contrasena)
        {
            usuario.IntentosFallidos++;

            if (usuario.Rol != Rol.Administrador && usuario.IntentosFallidos >= 2)
            {
                usuario.BloqueadoHasta = DateTime.Now.AddMinutes(3);

                await _servicioDeEmail.EnviarEmail(
                    usuario.Email,
                    $"USUARIO BLOQUEADO",
                    $"Le informamos que la cuenta del usuario {usuario.NombreUsuario} se encuentra bloqueada por 3 minutos. " +
                    $"Por favor ingrese a las {usuario.BloqueadoHasta:hh:mm} (Hora en la que se acaba el bloqueo)");
            }

            await _contexto.SaveChangesAsync();
            return (false, "Credenciales inválidas", null);
        }

        usuario.IntentosFallidos = 0;
        usuario.BloqueadoHasta = null;
        await _contexto.SaveChangesAsync();

        try
        {
            await _servicioDeEmail.EnviarEmail(
                usuario.Email,
                $"INICIO DE SESIÓN DEL USUARIO {usuario.NombreUsuario} ",
                $"Usted inició sesión el día {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:hh:mm}");
    }
        catch
        {

        }

        return (true, "Bienvenido", usuario.Rol);
    }

    public async Task<(bool Exitoso, string Mensaje)> CambiarContrasenaAsync(CambiarContrasenaDto request)
    {
        var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario);

        if (usuario == null) return (false, "El usuario no existe.");

        if (usuario.Contrasena != request.ContrasenaActual)
            return (false, "La contraseña actual es incorrecta.");

        if (usuario.Contrasena == request.ContrasenaNueva)
            return (false, "La nueva contraseña no puede ser igual a la actual.");

        usuario.Contrasena = request.ContrasenaNueva;
        await _contexto.SaveChangesAsync();

        await _servicioDeEmail.EnviarEmail(
                usuario.Email,
                $"CAMBIO DE CLAVE DEL USUARIO {usuario.NombreUsuario} ",
                $"Le informamos que la cuenta del usuario {usuario.NombreUsuario} ha cambiado su clave.");

        return (true, "Contraseña actualizada correctamente.");
    }
}