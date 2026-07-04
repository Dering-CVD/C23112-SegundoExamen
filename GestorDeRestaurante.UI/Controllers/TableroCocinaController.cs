using GestorDeRestaurante.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static GestorDeRestaurante.Dominio.Dtos.CocinaDto;

namespace GestorDeRestaurante.UI.Controllers
{
    [Authorize(Roles = "Cocina")]
    public class TableroCocinaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        private static readonly JsonSerializerOptions JsonOpciones = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public TableroCocinaController(IConfiguration configuration)
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

        // GET: TableroCocina
        public async Task<IActionResult> Index(string? buscarNombre = null, int? filtrarPedido = null, string? filtrarTipo = null)
        {
            var model = new TableroCocina();
            var httpClient = CrearHttpClient();

            // Guardar los filtros activos en el ViewData para mantenerlos visibles en los inputs de la vista
            ViewData["BuscarNombre"] = buscarNombre;
            ViewData["FiltrarPedido"] = filtrarPedido;
            ViewData["FiltrarTipo"] = filtrarTipo;

            try
            {
                // CORRECCIÓN AQUÍ: Cambiar 'buscarNombre=' por 'filtroNombre=' para que coincida con la API
                var queryParams = $"filtroNombre={Uri.EscapeDataString(buscarNombre ?? "")}&filtrarPedido={filtrarPedido}&filtrarTipo={filtrarTipo}";

                // 1. Obtener Pendientes
                var responsePendientes = await httpClient.GetAsync($"api/Cocina/ListarPedidosPendientesAsync?{queryParams}");
                if (responsePendientes.IsSuccessStatusCode)
                {
                    var json = await responsePendientes.Content.ReadAsStringAsync();
                    model.PlatillosPendientes = JsonSerializer.Deserialize<List<ListarPedidosPendientesDto>>(json, JsonOpciones) ?? new();
                }

                // 2. Obtener En Preparación
                var responseEnPrep = await httpClient.GetAsync($"api/Cocina/ListarPedidosEnPreparacionAsync?{queryParams}");
                if (responseEnPrep.IsSuccessStatusCode)
                {
                    var json = await responseEnPrep.Content.ReadAsStringAsync();
                    model.PlatillosEnPreparacion = JsonSerializer.Deserialize<List<ListarPedidosEnPreparacionDto>>(json, JsonOpciones) ?? new();
                }

                // 3. Obtener Atendidos
                var responseAtendidos = await httpClient.GetAsync($"api/Cocina/ListarPedidosAtendidosAsync?{queryParams}");
                if (responseAtendidos.IsSuccessStatusCode)
                {
                    var json = await responseAtendidos.Content.ReadAsStringAsync();
                    model.PlatillosAtendidos = JsonSerializer.Deserialize<List<ListarPedidosAtendidosDto>>(json, JsonOpciones) ?? new();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al conectar con el servicio de cocina: " + ex.Message;
            }

            return View(model);
        }

        // POST: TableroCocina/IniciarPreparacion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarPreparacion(int id)
        {
            var httpClient = CrearHttpClient();

            // Llamar al POST de tu API que realmente cambia el estado del pedido
            var response = await httpClient.PostAsync($"api/Cocina/IniciarPreparacionAsync?idDetalle={id}", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "No se pudo iniciar la preparación.";
            }
            else
            {
                TempData["SuccessMessage"] = "La preparación ha sido iniciada.";
            }

            // Retorna a la vista actual manteniendo los filtros si los hubiera
            return RedirectToAction(nameof(Index), new
            {
                buscarNombre = HttpContext.Request.Query["buscarNombre"],
                filtrarPedido = HttpContext.Request.Query["filtrarPedido"],
                filtrarTipo = HttpContext.Request.Query["filtrarTipo"]
            });
        }

        // POST: TableroCocina/MarcarAtendido/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarAtendido(int id, string observacion)
        {
            var httpClient = CrearHttpClient();
            // CORRECCIÓN: Ruta exacta del POST en tu API
            var url = $"api/Cocina/MarcarPedidoComoAtendidoAsync?idDetalle={id}";
            if (!string.IsNullOrEmpty(observacion))
            {
                url += $"&observaciones={Uri.EscapeDataString(observacion)}";
            }

            var response = await httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = !string.IsNullOrWhiteSpace(jsonRespuesta) ? jsonRespuesta : "No se pudo marcar como atendido.";
            }
            else
            {
                TempData["SuccessMessage"] = "El platillo ha sido despachado (Atendido).";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}