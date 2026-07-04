using GestorDeRestaurante.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers;

[Authorize(Roles = "Administrador")]
public class RecetasController : Controller
{
    private readonly string _apiBaseUrl;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RecetasController(IConfiguration configuration)
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
                response = await httpClient.GetAsync("api/Recetas/ObtengaLaListaDeRecetas");
            else
                response = await httpClient.GetAsync($"api/Recetas/ObtengaLaListaDeRecetasPorPlatillo?nombre={Uri.EscapeDataString(nombre)}");

            if (!await ValidarRespuestaAsync(response))
                return View(new List<Receta>());

            var json = await response.Content.ReadAsStringAsync();
            var recetas = JsonSerializer.Deserialize<List<Receta>>(json, JsonOpciones);
            return View(recetas ?? new List<Receta>());
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(new List<Receta>());
        }
    }

    public async Task<IActionResult> Crear()
    {
        await CargarPlatillosAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Receta receta)
    {
        ModelState.Remove("FechaDeCreacion");

        try
        {
            if (!ModelState.IsValid)
            {
                await CargarPlatillosAsync();
                return View(receta);
            }

            receta.FechaDeCreacion = DateTime.Now;

            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(receta);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("api/Recetas/AgregarReceta", content);

            if (!await ValidarRespuestaAsync(response))
            {
                await CargarPlatillosAsync();
                return View(receta);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            await CargarPlatillosAsync();
            return View(receta);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var httpClient = CrearHttpClient();

            var responseReceta = await httpClient.GetAsync($"api/Recetas/ObtengaLaRecetaPorId?id={id}");
            if (!await ValidarRespuestaAsync(responseReceta))
                return RedirectToAction(nameof(Index));

            var jsonReceta = await responseReceta.Content.ReadAsStringAsync();
            var receta = JsonSerializer.Deserialize<Receta>(jsonReceta, JsonOpciones);
            if (receta == null) return RedirectToAction(nameof(Index));

            var responseIngredientes = await httpClient.GetAsync($"api/Recetas/ObtengaIngredientesDeReceta?idReceta={id}");
            if (responseIngredientes.IsSuccessStatusCode)
            {
                var jsonIng = await responseIngredientes.Content.ReadAsStringAsync();
                ViewBag.Ingredientes = JsonSerializer.Deserialize<List<RecetaDetalleIngrediente>>(jsonIng, JsonOpciones)
                    ?? new List<RecetaDetalleIngrediente>();
            }

            return View(receta);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> AgregarIngrediente(int recetaId)
    {
        ViewBag.RecetaId = recetaId;
        await CargarIngredientesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarIngrediente(RecetaDetalleIngrediente recetaIngrediente)
    {
        ModelState.Remove("Obsvacion");

        try
        {
            var httpClient = CrearHttpClient();
            var json = JsonSerializer.Serialize(recetaIngrediente);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("api/Recetas/AgregarIngredienteAReceta", content);

            if (!await ValidarRespuestaAsync(response))
            {
                ViewBag.RecetaId = recetaIngrediente.IdReceta;
                await CargarIngredientesAsync();
                return View(recetaIngrediente);
            }

            return RedirectToAction("Details", new { id = recetaIngrediente.IdReceta });
        }
        catch (HttpRequestException)
        {
            ViewBag.Error = "El servicio no está disponible. Intente más tarde.";
            ViewBag.RecetaId = recetaIngrediente.IdReceta;
            await CargarIngredientesAsync();
            return View(recetaIngrediente);
        }
    }

    public async Task<IActionResult> EliminarIngrediente(int id, int recetaId)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.DeleteAsync($"api/Recetas/EliminarIngredienteDeReceta?id={id}");

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction("Details", new { id = recetaId });

            return RedirectToAction("Details", new { id = recetaId });
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "El servicio no está disponible. Intente más tarde.";
            return RedirectToAction("Details", new { id = recetaId });
        }
    }

    private async Task CargarPlatillosAsync()
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync("api/Menu/ObtengaLaListaDePlatillos");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var platillos = JsonSerializer.Deserialize<List<Platillo>>(json, JsonOpciones) ?? new List<Platillo>();

            ViewBag.Platillos = platillos
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToList();
        }
        catch { }
    }

    private async Task CargarIngredientesAsync()
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync("api/Recetas/ObtengaLaListaDeIngredientes");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var ingredientes = JsonSerializer.Deserialize<List<Ingrediente>>(json, JsonOpciones) ?? new List<Ingrediente>();

            ViewBag.IngredientesLista = ingredientes
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Nombre} - {i.UnidadDeMedida}"
                })
                .ToList();
        }
        catch { }
    }
}