using SistemaAlquilerPlaya.Dominio.Entidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SistemaAlquilerPlaya.UI.Servicios;

public class ServicioDeSesion : IServicioDeSesion
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServicioDeSesion(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task GenerarSesionAsync(string nombreUsuario, Rol rol)
    {
        var contexto = _httpContextAccessor.HttpContext;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nombreUsuario),
            new Claim(ClaimTypes.Role, rol.ToString())
        };

        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        await contexto!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
    }

    public async Task FinalizarSesionAsync()
    {
        var contexto = _httpContextAccessor.HttpContext;
        await contexto!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        contexto!.Session.Clear();
    }
}