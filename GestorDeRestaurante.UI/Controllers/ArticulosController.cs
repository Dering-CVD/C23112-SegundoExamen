using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaAlquilerPlaya.Dominio.Entidades;
using System.Net;
using System.Text.Json;

namespace SistemaAlquilerPlaya.UI.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]

    public class ArticulosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        private static readonly JsonSerializerOptions JsonOpciones = new()
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        public ArticulosController(IConfiguration configuration)
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
        public async Task<IActionResult> Index(string nombre)
        {
            try
            {
                var httpClient = CrearHttpClient();
                HttpResponseMessage response;

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    response = await httpClient.GetAsync("api/Articulo/ObtenerLaListaDeArticulos");
                }
                else
                {
                    response = await httpClient.GetAsync($"api/Articulo/ObtengaLaListaDeArticulosPorNombreDelCliente?nombreDelCliente={Uri.EscapeDataString(nombre)}");
                }
                if (!await ValidarRespuestaAsync(response))
                {
                    return View(new List<Articulo>());
                }
                var json = await response.Content.ReadAsStringAsync();
                var articulos = JsonSerializer.Deserialize<List<Articulo>>(json, JsonOpciones);
                return View(articulos ?? new List<Articulo>());
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(new List<Articulo>());
            }
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Articulo articulo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(articulo.NombreDelCliente))
                {
                    articulo.NombreDelCliente = "Cliente genérico";
                }
                ModelState.Remove("NombreDelCliente");

                if (!ModelState.IsValid)
                    return View(articulo);

                var httpClient = CrearHttpClient();
                var json = JsonSerializer.Serialize(articulo);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/Articulo/CrearArticulo", content);

                if (!await ValidarRespuestaAsync(response))
                    return View(articulo);

                TempData["SuccessMessage"] = "Articulo creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(articulo);
            }
        }
        public async Task<IActionResult> Detalles(int id, string nombrePlatillo = "", string nombreAgregado = "")
        {
            try
            {
                var httpClient = CrearHttpClient();
                var responseArticulo = await httpClient.GetAsync($"api/Articulo/ObtenerArticuloPorId?id={id}");
                if (!await ValidarRespuestaAsync(responseArticulo))
                    return RedirectToAction(nameof(Index));

                var jsonArticulo = await responseArticulo.Content.ReadAsStringAsync();
                var articulo = JsonSerializer.Deserialize<Articulo>(jsonArticulo, JsonOpciones);

                if (articulo == null)
                    return RedirectToAction(nameof(Index));

                var response = await httpClient.GetAsync($"api/Articulo/ObtenerDetalleDeArticuloFiltradoPorNombre?idArticulo={id}&nombrePlatillo={nombreAgregado}");

                var jsonContent = await response.Content.ReadAsStringAsync();
                List<ArticuloDetalle> detalles = new List<ArticuloDetalle>();
                if (response.IsSuccessStatusCode)
                {
                    var jsonDetalles = await response.Content.ReadAsStringAsync();
                    detalles = JsonSerializer.Deserialize<List<ArticuloDetalle>>(jsonDetalles, JsonOpciones) ?? new List<ArticuloDetalle>();
                }
                else
                {
                    var errorReal = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Fallo en la API: Status {response.StatusCode} | Detalles: {errorReal}");
                }

                var responsePlatillos = await httpClient.GetAsync($"api/Articulo/ObtenerPlatillosDisponiblesFiltradoPorNombre?nombrePlatillo={Uri.EscapeDataString(nombrePlatillo)}");
                List<Platillo> platillos = new List<Platillo>();
                if (responsePlatillos.IsSuccessStatusCode)
                {
                    var jsonPlatillos = await responsePlatillos.Content.ReadAsStringAsync();
                    platillos = JsonSerializer.Deserialize<List<Platillo>>(jsonPlatillos, JsonOpciones) ?? new List<Platillo>();
                }

                var responseDesglose = await httpClient.GetAsync($"api/Articulo/CalcularDesgloseDeArticulo?idArticulo={id}");
                decimal subtotal = 0, impuesto = 0, totalConImpuesto = 0;
                if (responseDesglose.IsSuccessStatusCode)
                {
                    var jsonDesglose = await responseDesglose.Content.ReadAsStringAsync();
                    var desglose = JsonSerializer.Deserialize<DesgloseArticuloDto>(jsonDesglose, JsonOpciones);
                    if (desglose != null)
                    {
                        subtotal = desglose.Subtotal;
                        impuesto = desglose.Impuesto;
                        totalConImpuesto = desglose.Total;
                    }
                }

                ViewBag.Articulo = articulo;
                ViewBag.DetallesArticulo = detalles;
                ViewBag.PlatillosDisponibles = platillos;
                ViewBag.NombreFiltro = nombrePlatillo;
                ViewBag.NombreAgregadoFiltro = nombreAgregado;

                ViewBag.Subtotal = subtotal;
                ViewBag.Impuesto = impuesto;
                ViewBag.TotalGeneralArticulo = totalConImpuesto;

                return View();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detalles(int idArticulo, int idPlatillo, int cantidad, string? observaciones)
        {
            try
            {
                var httpClient = CrearHttpClient();

                var url = $"api/Articulo/AgregarPlatilloAlArticulo?idPlatillo={idPlatillo}&idArticulo={idArticulo}&cantidad={cantidad}";
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
                        : "No se pudo agregar el platillo. Verifique el estado del articulo.";

                    return RedirectToAction(nameof(Detalles), new { id = idArticulo, nombreAgregado = "" });
                }

                TempData["Exito"] = "Platillo agregado exitosamente.";
                return RedirectToAction(nameof(Detalles), new { id = idArticulo, nombreAgregado = "" });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "El servicio no está disponible. Intente más tarde.";
                return RedirectToAction(nameof(Detalles), new { id = idArticulo, nombreAgregado = "" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPlatillo(int idArticulo, int idArticuloDetalle)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = $"api/Articulo/EliminarPlatilloDelArticulo?idArticuloDetalle={idArticuloDetalle}";

                var response = await httpClient.DeleteAsync(url);

                if (!await ValidarRespuestaAsync(response))
                {
                    TempData["Error"] = "No se pudo eliminar el platillo del articulo.";
                    return RedirectToAction(nameof(Detalles), new { id = idArticulo });
                }

                TempData["Exito"] = "Platillo eliminado exitosamente.";
                return RedirectToAction(nameof(Detalles), new { id = idArticulo });
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "El servicio no está disponible. Intente más tarde.";
                return RedirectToAction(nameof(Detalles), new { id = idArticulo });
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var response = await httpClient.GetAsync($"api/Articulo/ObtenerArticuloPorId?id={id}");

                if (!await ValidarRespuestaAsync(response))
                    return RedirectToAction(nameof(Index));

                var json = await response.Content.ReadAsStringAsync();
                var articulo = JsonSerializer.Deserialize<Articulo>(json, JsonOpciones);

                if (articulo == null)
                    return NotFound();

                return View(articulo);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Articulo articulo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(articulo);

                var httpClient = CrearHttpClient();
                var json = JsonSerializer.Serialize(articulo);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync("api/Articulo/EditarArticulo", content);

                if (!await ValidarRespuestaAsync(response))
                {
                    ModelState.AddModelError("", "No se pudieron guardar los cambios del articulo.");
                    return View(articulo);
                }

                TempData["SuccessMessage"] = "articulo actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(articulo);
            }
        }
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var response = await httpClient.GetAsync($"api/Articulo/ObtenerArticuloPorId?id={id}");

                if (!await ValidarRespuestaAsync(response))
                {
                    TempData["Error"] = "El articulo especificado no existe.";
                    return RedirectToAction(nameof(Index));
                }

                var json = await response.Content.ReadAsStringAsync();
                var articulo = JsonSerializer.Deserialize<Articulo>(json, JsonOpciones);

                if (articulo == null)
                    return RedirectToAction(nameof(Index));

                if (articulo.Estado == SistemaAlquilerPlaya.Dominio.Entidades.EstadoArticulo.Facturado ||
                    articulo.Estado == SistemaAlquilerPlaya.Dominio.Entidades.EstadoArticulo.Cancelado)
                {
                    TempData["Error"] = $"No se puede cancelar un articulo con estado {articulo.Estado}.";
                    return RedirectToAction(nameof(Index));
                }

                return View(articulo);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "El servicio no está disponible. Intente más tarde.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string motivoDeLaCancelacion, bool ignorarAdvertenciaPlatillosAtendidos)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var url = $"api/Articulo/CancelarArticulo?idArticulo={id}" +
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
                        ModelState.AddModelError(string.Empty, mensajeError ?? "No se pudo cancelar el articulo.");
                    }
                    var responseArticulo = await httpClient.GetAsync($"api/Articulo/ObtenerArticuloPorId?id={id}");
                    var jsonArticulo = await responseArticulo.Content.ReadAsStringAsync();
                    var articulo = JsonSerializer.Deserialize<Articulo>(jsonArticulo, JsonOpciones);

                    return View(articulo); 
                }

                TempData["Exito"] = "El articulo ha sido cancelado exitosamente.";
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