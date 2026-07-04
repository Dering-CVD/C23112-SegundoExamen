using GestorDeRestaurante.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers;

[Authorize(Roles = "Administrador")]
public class IngredientesController : Controller
{
    private readonly string _apiBaseUrl;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IngredientesController(IConfiguration configuration)
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

    public async Task<IActionResult> Index(string nombre)
    {
        try
        {
            var httpClient = CrearHttpClient();
            HttpResponseMessage response;

            if (string.IsNullOrWhiteSpace(nombre))
                response = await httpClient.GetAsync("api/Ingredientes/ObtenerIngredientes");
            else
                response = await httpClient.GetAsync($"api/Ingredientes/BuscarPorNombre?nombre={Uri.EscapeDataString(nombre)}");

            if (!await ValidarRespuestaAsync(response))
                return View(new List<Ingrediente>());

            var json = await response.Content.ReadAsStringAsync();
            var ingredientes = JsonSerializer.Deserialize<List<Ingrediente>>(json, JsonOpciones);
            return View(ingredientes ?? new List<Ingrediente>());
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(new List<Ingrediente>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Ingrediente ingrediente)
    {
        if (!ModelState.IsValid) return View(ingrediente);

        try
        {
            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(ingrediente);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("api/Ingredientes/Agregar", content);

            if (!await ValidarRespuestaAsync(response))
                return View(ingrediente);

            TempData["SuccessMessage"] = "Ingrediente agregado exitosamente.";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(ingrediente);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync($"api/Ingredientes/ObtenerPorId?id={id}");

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var ingrediente = JsonSerializer.Deserialize<Ingrediente>(json, JsonOpciones);

            if (ingrediente == null)
                return RedirectToAction("Index");

            return View(ingrediente);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Ingrediente ingrediente)
    {
        if (!ModelState.IsValid) return View(ingrediente);

        try
        {
            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(ingrediente);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync("api/Ingredientes/Editar", content);

            if (!await ValidarRespuestaAsync(response))
                return View(ingrediente);

            TempData["SuccessMessage"] = "Ingrediente actualizado exitosamente.";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(ingrediente);
        }
    }

    public async Task<IActionResult> AjustarInventario(int id)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync($"api/Ingredientes/ObtenerPorId?id={id}");

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction("Index");

            var json = await response.Content.ReadAsStringAsync();
            var ingrediente = JsonSerializer.Deserialize<Ingrediente>(json, JsonOpciones);

            if (ingrediente == null)
                return RedirectToAction("Index");

            return View(ingrediente);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AjustarInventario(int id, double? cantidad, string tipo)
    {
        try
        {
            if (cantidad == null || cantidad <= 0)
            {
                ModelState.AddModelError("", "La cantidad para el ajuste es obligatoria y debe ser mayor a cero.");
                var ingrediente = await ObtenerIngredientePorIdAsync(id);
                return View(ingrediente);
            }

            var httpClient = CrearHttpClient();
            var response = await httpClient.PutAsync(
                $"api/Ingredientes/AjustarInventario?id={id}&cantidad={cantidad}&tipo={Uri.EscapeDataString(tipo)}", null);

            if (!await ValidarRespuestaAsync(response))
            {
                var ingrediente = await ObtenerIngredientePorIdAsync(id);
                return View(ingrediente);
            }

            TempData["SuccessMessage"] = "Inventario ajustado exitosamente.";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Activar(int id)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.PutAsync($"api/Ingredientes/Activar?id={id}", null);

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction("Index");

            TempData["SuccessMessage"] = "Ingrediente activado exitosamente.";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Inactivar(int id)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.PutAsync($"api/Ingredientes/Inactivar?id={id}", null);

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction("Index");

            TempData["SuccessMessage"] = "Ingrediente inactivado exitosamente.";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return RedirectToAction("Index");
        }
    }

    private async Task<Ingrediente?> ObtenerIngredientePorIdAsync(int id)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync($"api/Ingredientes/ObtenerPorId?id={id}");

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Ingrediente>(json, JsonOpciones);
        }
        catch
        {
            return null;
        }
    }
}