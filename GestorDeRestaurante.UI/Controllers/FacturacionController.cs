using GestorDeRestaurante.Dominio.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers;

[Authorize(Roles = "Administrador, Empleado")]
public class FacturacionController : Controller
{
    private readonly string _apiBaseUrl;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions JsonOpciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public FacturacionController(IConfiguration configuration)
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

    public async Task<IActionResult> Index()
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync("api/Facturacion/ObtenerPedidosDisponiblesParaFacturar");

            if (!await ValidarRespuestaAsync(response))
                return View(new List<PedidoFacturableDto>());

            var json = await response.Content.ReadAsStringAsync();
            var pedidos = JsonSerializer.Deserialize<List<PedidoFacturableDto>>(json, JsonOpciones);
            return View(pedidos ?? new List<PedidoFacturableDto>());
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
            return View(new List<PedidoFacturableDto>());
        }
    }

    public async Task<IActionResult> GenerarFactura(int idPedido)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync($"api/Facturacion/PrepararFactura?idPedido={idPedido}");

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction(nameof(Index));

            var json = await response.Content.ReadAsStringAsync();
            var factura = JsonSerializer.Deserialize<FacturaDto>(json, JsonOpciones);

            if (factura == null)
            {
                TempData["MensajeError"] = "El pedido no existe o no está disponible para facturar.";
                return RedirectToAction(nameof(Index));
            }

            return View(factura);
        }
        catch (HttpRequestException)
        {
            TempData["MensajeError"] = "El servicio no está disponible. Intente más tarde.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarFacturaConfirmada(int idPedido)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.PostAsync($"api/Facturacion/GenerarFactura?idPedido={idPedido}", null);

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction(nameof(Index));

            TempData["MensajeExito"] = "Factura registrada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["MensajeError"] = "El servicio no está disponible. Intente más tarde.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> DetalleFactura(int idPedido)
    {
        try
        {
            var httpClient = CrearHttpClient();
            var response = await httpClient.GetAsync($"api/Facturacion/PrepararFactura?idPedido={idPedido}");

            if (!await ValidarRespuestaAsync(response))
                return RedirectToAction(nameof(Index));

            var json = await response.Content.ReadAsStringAsync();
            var factura = JsonSerializer.Deserialize<FacturaDto>(json, JsonOpciones);

            if (factura == null)
                return NotFound();

            return View(factura);
        }
        catch (HttpRequestException)
        {
            TempData["MensajeError"] = "El servicio no está disponible. Intente más tarde.";
            return RedirectToAction(nameof(Index));
        }
    }
}