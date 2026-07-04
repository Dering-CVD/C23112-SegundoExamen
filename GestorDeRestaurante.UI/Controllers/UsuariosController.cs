using GestorDeRestaurante.Dominio.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly string _apiBaseUrl;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UsuariosController(IConfiguration configuration)
    {
        _apiBaseUrl = configuration["BaseUrl"]!;
        _apiKey = configuration["ApiKey"]!;
    }

    private HttpClient CrearHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_apiBaseUrl);
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        return httpClient;
    }

    private async Task<bool> ValidarRespuestaAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return true;

        var mensaje = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "Error de autenticación: API Key no proporcionada.",
            HttpStatusCode.Forbidden => "Error de autenticación: API Key inválida.",
            _ => await response.Content.ReadAsStringAsync()
        };

        ModelState.AddModelError("", mensaje);
        return false;
    }

    public async Task<IActionResult> Index(string? filtro)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var url = string.IsNullOrWhiteSpace(filtro)
                ? "api/Usuarios/ListarUsuarios"
                : $"api/Usuarios/ListarUsuarios?filtroNombre={Uri.EscapeDataString(filtro)}";

            var response = await httpClient.GetAsync(url);

            if (!await ValidarRespuestaAsync(response))
                return View(new List<ListarUsuarioDto>());

            var json = await response.Content.ReadAsStringAsync();
            var usuarios = JsonSerializer.Deserialize<List<ListarUsuarioDto>>(json, JsonOpciones);
            return View(usuarios ?? new List<ListarUsuarioDto>());
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(new List<ListarUsuarioDto>());
        }
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearUsuarioDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        try
        {
            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("api/Usuarios/CrearUsuario", content);

            if (!await ValidarRespuestaAsync(response))
                return View(dto);

            var mensaje = await response.Content.ReadAsStringAsync();
            TempData["SuccessMessage"] = mensaje;
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(dto);
        }
    }

    public async Task<IActionResult> Editar(int id)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync($"api/Usuarios/ObtenerUsuarioAEditar?id={id}");

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction(nameof(Index));

            var json = await response.Content.ReadAsStringAsync();
            var usuarioDto = JsonSerializer.Deserialize<EditarUsuarioDto>(json, JsonOpciones);

            if (usuarioDto == null)
            {
                TempData["ErrorMessage"] = "El usuario solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuarioDto);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, EditarUsuarioDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        try
        {
            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"api/Usuarios/EditarUsuario?id={id}", content);

            if (!await ValidarRespuestaAsync(response))
                return View(dto);

            var mensaje = await response.Content.ReadAsStringAsync();
            TempData["SuccessMessage"] = mensaje;
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(dto);
        }
    }
}