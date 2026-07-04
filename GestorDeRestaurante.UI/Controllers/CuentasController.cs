using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.UI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers;

public class CuentasController : Controller
{
    private readonly string _apiBaseUrl;
    private readonly string _apiKey;
    private readonly IServicioDeSesion _servicioDeSesion;

    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CuentasController(IConfiguration configuration, IServicioDeSesion servicioDeSesion)
    {
        _apiBaseUrl = configuration["BaseUrl"]!;
        _apiKey = configuration["ApiKey"]!;
        _servicioDeSesion = servicioDeSesion;
    }

    private HttpClient CrearHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_apiBaseUrl);
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        return httpClient;
    }

    private async Task<string?> ValidarRespuestaAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return null;

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "Error de autenticación: API Key no proporcionada.",
            HttpStatusCode.Forbidden => "Error de autenticación: API Key inválida.",
            _ => await response.Content.ReadAsStringAsync()
        };
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto request)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("api/Cuentas/ValidarCredenciales", content);

            var error = await ValidarRespuestaAsync(response);
            if (error != null)
            {
                ViewBag.Error = error;
                return View();
            }

            var body = await response.Content.ReadAsStringAsync();
            using var documento = JsonDocument.Parse(body);
            var rolValue = documento.RootElement.GetProperty("rol").GetInt32();

            await _servicioDeSesion.GenerarSesionAsync(request.NombreUsuario, (Rol)rolValue);
            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View();
        }
    }

    [Authorize(Roles = "Administrador, Empleado, Cocina")]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _servicioDeSesion.FinalizarSesionAsync();
        return RedirectToAction("Login");
    }

    [Authorize(Roles = "Administrador, Empleado, Cocina")]
    [HttpGet]
    public IActionResult CambiarContrasena()
    {
        var modelo = new CambiarContrasenaDto { NombreUsuario = User.Identity!.Name! };
        return View(modelo);
    }

    [Authorize(Roles = "Administrador, Empleado, Cocina")]
    [HttpPost]
    public async Task<IActionResult> CambiarContrasena(CambiarContrasenaDto request)
    {
        if (!ModelState.IsValid) return View(request);

        try
        {
            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync("api/Cuentas/CambiarContrasena", content);

            var error = await ValidarRespuestaAsync(response);
            if (error != null)
            {
                ViewBag.Error = error;
                return View();
            }

            var mensaje = await response.Content.ReadAsStringAsync();
            TempData["SuccessMessage"] = mensaje;
            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(request);
        }
    }
}