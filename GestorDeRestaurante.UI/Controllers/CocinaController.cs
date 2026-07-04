using GestorDeRestaurante.Dominio.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GestorDeRestaurante.UI.Controllers
{
    [Authorize(Roles = "Cocina")]
    public class CocinaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        private static readonly JsonSerializerOptions JsonOpciones = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public CocinaController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["BaseUrl"]!;
            _apiKey = _configuration["ApiKey"]!;
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
                System.Net.HttpStatusCode.Unauthorized => "No está autorizado para realizar esta acción.",
                System.Net.HttpStatusCode.Forbidden => "Acceso denegado a la API.",
                System.Net.HttpStatusCode.NotFound => "El recurso solicitado en la API no fue encontrado (404).",
                _ => $"Error en el servicio de cocina: {response.ReasonPhrase}"
            };

            TempData["ErrorMessage"] = mensaje;
            return false;
        }

        // GET: Cocina 
        public async Task<IActionResult> Index(string buscar)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = string.IsNullOrEmpty(buscar)
                    ? "api/Cocina/ListarPedidosPendientesAsync"
                    : $"api/Cocina/ListarPedidosPendientesAsync?filtroNombre={Uri.EscapeDataString(buscar)}";

                var response = await httpClient.GetAsync(url);
                List<CocinaDto.ListarPedidosPendientesDto> pendientes = new List<CocinaDto.ListarPedidosPendientesDto>();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    pendientes = JsonSerializer.Deserialize<List<CocinaDto.ListarPedidosPendientesDto>>(json, JsonOpciones)
                                 ?? new List<CocinaDto.ListarPedidosPendientesDto>();
                }
                else
                {
                    await ValidarRespuestaAsync(response);
                }

                return View(pendientes);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "El servicio de cocina no está disponible.";
                return View(new List<CocinaDto.ListarPedidosPendientesDto>());
            }
        }

        public async Task<IActionResult> EnPreparacion(string buscar)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = string.IsNullOrEmpty(buscar)
                    ? "api/Cocina/ListarPedidosEnPreparacionAsync"
                    : $"api/Cocina/ListarPedidosEnPreparacionAsync?filtroNombre={Uri.EscapeDataString(buscar)}";

                var response = await httpClient.GetAsync(url);
                List<CocinaDto.ListarPedidosEnPreparacionDto> preparando = new List<CocinaDto.ListarPedidosEnPreparacionDto>();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    preparando = JsonSerializer.Deserialize<List<CocinaDto.ListarPedidosEnPreparacionDto>>(json, JsonOpciones)
                                 ?? new List<CocinaDto.ListarPedidosEnPreparacionDto>();
                }
                else
                {
                    await ValidarRespuestaAsync(response);
                }

                return View(preparando);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "El servicio de cocina no está disponible.";
                return View(new List<CocinaDto.ListarPedidosEnPreparacionDto>());
            }
        }

        // GET: Atendidos
        public async Task<IActionResult> Atendidos(string buscar)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = string.IsNullOrEmpty(buscar)
                    ? "api/Cocina/ListarPedidosAtendidosAsync"
                    : $"api/Cocina/ListarPedidosAtendidosAsync?filtroNombre={Uri.EscapeDataString(buscar)}";

                var response = await httpClient.GetAsync(url);
                List<CocinaDto.ListarPedidosAtendidosDto> atendidos = new List<CocinaDto.ListarPedidosAtendidosDto>();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    atendidos = JsonSerializer.Deserialize<List<CocinaDto.ListarPedidosAtendidosDto>>(json, JsonOpciones)
                                ?? new List<CocinaDto.ListarPedidosAtendidosDto>();
                }
                else
                {
                    await ValidarRespuestaAsync(response);
                }

                return View(atendidos);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "El servicio de cocina no está disponible.";
                return View(new List<CocinaDto.ListarPedidosAtendidosDto>());
            }
        }

        // GET: ObtenerDetallesReceta
        [HttpGet]
        public async Task<IActionResult> ObtenerDetallesReceta(int idDetalle)
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync($"api/Cocina/ObtenerDetallesRecetaAsync?idDetalle={idDetalle}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

        // POST: IniciarPreparacion
        [HttpPost]
        public async Task<IActionResult> IniciarPreparacion(int id)
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.PostAsync($"api/Cocina/IniciarPreparacionAsync?idDetalle={id}", null);
            if (!response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();

                try
                {
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(jsonRespuesta);
                    string mensajeError = errorObj.GetProperty("message").GetString() ?? "Error al iniciar preparación.";

                    TempData["ErrorMessage"] = mensajeError;
                }
                catch
                {
                    TempData["ErrorMessage"] = "No hay suficiente inventario para este platillo.";
                }

                return RedirectToAction("Index"); 
            }

            TempData["SuccessMessage"] = "Preparación iniciada correctamente.";
            return RedirectToAction("Index");
        }

        // POST: MarcarAtendido
        [HttpPost]
        public async Task<IActionResult> MarcarAtendido(int id, string observacion)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = $"api/Cocina/MarcarPedidoComoAtendidoAsync?idDetalle={id}";
                if (!string.IsNullOrEmpty(observacion))
                {
                    url += $"&observaciones={Uri.EscapeDataString(observacion)}";
                }

                var response = await httpClient.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    var jsonRespuesta = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = !string.IsNullOrWhiteSpace(jsonRespuesta) ? jsonRespuesta : "No se pudo marcar el platillo como atendido.";
                    return RedirectToAction("EnPreparacion");
                }
                try
                {
                    await httpClient.GetAsync($"api/Pedido/ActualizarEstadoDePedido?idPedidoDetalle={id}");
                }
                catch { }

                TempData["SuccessMessage"] = "El platillo ha sido despachado (Atendido).";
                return RedirectToAction("Atendidos");
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "El servicio de cocina no está disponible.";
                return RedirectToAction("EnPreparacion");
            }
        }
    }
}