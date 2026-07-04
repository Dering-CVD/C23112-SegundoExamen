using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]

    public class PedidoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        private static readonly JsonSerializerOptions JsonOpciones = new()
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        public PedidoController(IConfiguration configuration)
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
                HttpStatusCode.Unauthorized => "Error de autenticación: API Key no proporcionada.",
                HttpStatusCode.Forbidden => "Error de autenticación: API Key inválida.",
                _ => await response.Content.ReadAsStringAsync()
            };

            ModelState.AddModelError("", mensaje);
            return false;
        }

        // GET: PedidoController
        public async Task<IActionResult> Index(string nombre)
        {
            try
            {
                var httpClient = CrearHttpClient();
                HttpResponseMessage response;

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    response = await httpClient.GetAsync("api/Pedido/ObtenerLaListaDePedidos");
                }
                else
                {
                    response = await httpClient.GetAsync($"api/Pedido/ObtengaLaListaDePedidosPorNombreDelCliente?nombreDelCliente={Uri.EscapeDataString(nombre)}");
                }
                if (!await ValidarRespuestaAsync(response))
                {
                    return View(new List<Pedido>());
                }
                var json = await response.Content.ReadAsStringAsync();
                var pedidos = JsonSerializer.Deserialize<List<Pedido>>(json, JsonOpciones);
                return View(pedidos ?? new List<Pedido>());
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(new List<Pedido>());
            }

        }

        // GET: PedidoController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PedidoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pedido pedido)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pedido.NombreDelCliente))
                {
                    pedido.NombreDelCliente = "Cliente genérico";
                }
                ModelState.Remove("NombreDelCliente");

                if (!ModelState.IsValid)
                    return View(pedido);

                var httpClient = CrearHttpClient();
                var json = JsonSerializer.Serialize(pedido);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/Pedido/CrearPedido", content);

                if (!await ValidarRespuestaAsync(response))
                    return View(pedido);

                TempData["SuccessMessage"] = "Pedido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(pedido);
            }
        }
        // GET: PedidoController/Detalles/5
        public async Task<IActionResult> Detalles(int id, string nombrePlatillo = "", string nombreAgregado = "")
        {
            try
            {
                var httpClient = CrearHttpClient();
                var responsePedido = await httpClient.GetAsync($"api/Pedido/ObtenerPedidoPorId?id={id}");
                if (!await ValidarRespuestaAsync(responsePedido))
                    return RedirectToAction(nameof(Index));

                var jsonPedido = await responsePedido.Content.ReadAsStringAsync();
                var pedido = JsonSerializer.Deserialize<Pedido>(jsonPedido, JsonOpciones);

                if (pedido == null)
                    return RedirectToAction(nameof(Index));

                var response = await httpClient.GetAsync($"api/Pedido/ObtenerDetalleDePedidoFiltradoPorNombre?idPedido={id}&nombrePlatillo={nombreAgregado}");

                var jsonContent = await response.Content.ReadAsStringAsync();
                List<PedidoDetalle> detalles = new List<PedidoDetalle>();
                if (response.IsSuccessStatusCode)
                {
                    var jsonDetalles = await response.Content.ReadAsStringAsync();
                    detalles = JsonSerializer.Deserialize<List<PedidoDetalle>>(jsonDetalles, JsonOpciones) ?? new List<PedidoDetalle>();
                }
                else
                {
                    var errorReal = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Fallo en la API: Status {response.StatusCode} | Detalles: {errorReal}");
                }

                var responsePlatillos = await httpClient.GetAsync($"api/Pedido/ObtenerPlatillosDisponiblesFiltradoPorNombre?nombrePlatillo={Uri.EscapeDataString(nombrePlatillo)}");
                List<Platillo> platillos = new List<Platillo>();
                if (responsePlatillos.IsSuccessStatusCode)
                {
                    var jsonPlatillos = await responsePlatillos.Content.ReadAsStringAsync();
                    platillos = JsonSerializer.Deserialize<List<Platillo>>(jsonPlatillos, JsonOpciones) ?? new List<Platillo>();
                }

                var responseDesglose = await httpClient.GetAsync($"api/Pedido/CalcularDesgloseDePedido?idPedido={id}");
                decimal subtotal = 0, impuesto = 0, totalConImpuesto = 0;
                if (responseDesglose.IsSuccessStatusCode)
                {
                    var jsonDesglose = await responseDesglose.Content.ReadAsStringAsync();
                    var desglose = JsonSerializer.Deserialize<DesglosePedidoDto>(jsonDesglose, JsonOpciones);
                    if (desglose != null)
                    {
                        subtotal = desglose.Subtotal;
                        impuesto = desglose.Impuesto;
                        totalConImpuesto = desglose.Total;
                    }
                }

                ViewBag.Pedido = pedido;
                ViewBag.DetallesPedido = detalles;
                ViewBag.PlatillosDisponibles = platillos;
                ViewBag.NombreFiltro = nombrePlatillo;
                ViewBag.NombreAgregadoFiltro = nombreAgregado;

                ViewBag.Subtotal = subtotal;
                ViewBag.Impuesto = impuesto;
                ViewBag.TotalGeneralPedido = totalConImpuesto;

                return View();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PedidoController/Detalles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detalles(int idPedido, int idPlatillo, int cantidad, string? observaciones)
        {
            try
            {
                var httpClient = CrearHttpClient();

                var url = $"api/Pedido/AgregarPlatilloAlPedido?idPlatillo={idPlatillo}&idPedido={idPedido}&cantidad={cantidad}";
                if (!string.IsNullOrWhiteSpace(observaciones))
                {
                    url += $"&observaciones={Uri.EscapeDataString(observaciones)}";
                }

                var response = await httpClient.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    var mensajeError = await response.Content.ReadAsStringAsync();

                    TempData["Error"] = !string.IsNullOrWhiteSpace(mensajeError)
                        ? mensajeError
                        : "No se pudo agregar el platillo. Verifique el estado del pedido.";

                    return RedirectToAction(nameof(Detalles), new { id = idPedido, nombreAgregado = "" });
                }

                TempData["Exito"] = "Platillo agregado exitosamente.";
                return RedirectToAction(nameof(Detalles), new { id = idPedido, nombreAgregado = "" });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "El servicio no está disponible. Intente más tarde.";
                return RedirectToAction(nameof(Detalles), new { id = idPedido, nombreAgregado = "" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPlatillo(int idPedido, int idPedidoDetalle)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = $"api/Pedido/EliminarPlatilloDelPedido?idPedidoDetalle={idPedidoDetalle}";

                var response = await httpClient.DeleteAsync(url);

                if (!await ValidarRespuestaAsync(response))
                {
                    TempData["Error"] = "No se pudo eliminar el platillo del pedido.";
                    return RedirectToAction(nameof(Detalles), new { id = idPedido });
                }

                TempData["Exito"] = "Platillo eliminado exitosamente.";
                return RedirectToAction(nameof(Detalles), new { id = idPedido });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "El servicio no está disponible. Intente más tarde.";
                return RedirectToAction(nameof(Detalles), new { id = idPedido });
            }
        }
        // GET: PedidoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var response = await httpClient.GetAsync($"api/Pedido/ObtenerPedidoPorId?id={id}");

                if (!await ValidarRespuestaAsync(response))
                    return RedirectToAction(nameof(Index));

                var json = await response.Content.ReadAsStringAsync();
                var pedido = JsonSerializer.Deserialize<Pedido>(json, JsonOpciones);

                if (pedido == null)
                    return NotFound();

                return View(pedido);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PedidoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Pedido pedido)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(pedido);

                var httpClient = CrearHttpClient();
                var json = JsonSerializer.Serialize(pedido);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync("api/Pedido/EditarPedido", content);

                if (!await ValidarRespuestaAsync(response))
                {
                    ModelState.AddModelError("", "No se pudieron guardar los cambios del pedido.");
                    return View(pedido);
                }

                TempData["SuccessMessage"] = "Pedido actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(pedido);
            }
        }

        // GET: PedidoController/Cancel/5
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var response = await httpClient.GetAsync($"api/Pedido/ObtenerPedidoPorId?id={id}");

                if (!await ValidarRespuestaAsync(response))
                {
                    TempData["Error"] = "El pedido especificado no existe.";
                    return RedirectToAction(nameof(Index));
                }

                var json = await response.Content.ReadAsStringAsync();
                var pedido = JsonSerializer.Deserialize<Pedido>(json, JsonOpciones);

                if (pedido == null)
                    return RedirectToAction(nameof(Index));

                if (pedido.Estado == GestorDeRestaurante.Dominio.Entidades.EstadoPedido.Facturado ||
                    pedido.Estado == GestorDeRestaurante.Dominio.Entidades.EstadoPedido.Cancelado)
                {
                    TempData["Error"] = $"No se puede cancelar un pedido con estado {pedido.Estado}.";
                    return RedirectToAction(nameof(Index));
                }

                return View(pedido);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "El servicio no está disponible. Intente más tarde.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PedidoController/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string motivoDeLaCancelacion, bool ignorarAdvertenciaPlatillosAtendidos)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = $"api/Pedido/CancelarPedido?idPedido={id}" +
                          $"&motivoDeLaCancelacion={Uri.EscapeDataString(motivoDeLaCancelacion)}" +
                          $"&ignorarAdvertenciaPlatillosAtendidos={ignorarAdvertenciaPlatillosAtendidos.ToString().ToLower()}";

                var response = await httpClient.PutAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    var mensajeError = await response.Content.ReadAsStringAsync();
                    if (mensajeError != null && mensajeError.Contains("ADVERTENCIA"))
                    {
                        ViewBag.MostrarAdvertenciaPlatillos = true;
                        ViewBag.MensajeAdvertencia = mensajeError;
                    }
                    else if (string.IsNullOrWhiteSpace(motivoDeLaCancelacion))
                    {
                        ModelState.AddModelError("motivoDeLaCancelacion", "El motivo de la cancelación es requerido.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, mensajeError ?? "No se pudo cancelar el pedido.");
                    }
                    var responsePedido = await httpClient.GetAsync($"api/Pedido/ObtenerPedidoPorId?id={id}");
                    var jsonPedido = await responsePedido.Content.ReadAsStringAsync();
                    var pedido = JsonSerializer.Deserialize<Pedido>(jsonPedido, JsonOpciones);

                    return View(pedido);
                }

                TempData["Exito"] = "El pedido ha sido cancelado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "El servicio no está disponible. Intente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}