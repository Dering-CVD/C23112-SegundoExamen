using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HistorialController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        private static readonly JsonSerializerOptions JsonOpciones = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HistorialController(IConfiguration configuration)
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
                System.Net.HttpStatusCode.NotFound => "El recurso solicitado no fue encontrado en el servidor.",
                _ => $"Error en el servicio: {response.ReasonPhrase}"
            };

            TempData["Error"] = mensaje;
            return false;
        }
        /// GET: Historial 
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// POST: Historial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DateTime fechaInicial, DateTime fechaFinal, string? estado)
        {
            try
            {
                if (fechaInicial > fechaFinal)
                {
                    ModelState.AddModelError("fechaInicial", "La fecha inicial no puede ser posterior a la fecha final.");
                    return View();
                }

                var httpClient = CrearHttpClient();

                var responsePedidos = await httpClient.GetAsync($"api/Pedido/ObtenerPedidosPorRangoDeFechas?fechaInicial={fechaInicial:yyyy-MM-dd}&fechaFinal={fechaFinal:yyyy-MM-dd}");

                if (!await ValidarRespuestaAsync(responsePedidos))
                {
                    return View();
                }

                var jsonPedidos = await responsePedidos.Content.ReadAsStringAsync();
                var pedidos = JsonSerializer.Deserialize<List<Pedido>>(jsonPedidos, JsonOpciones) ?? new List<Pedido>();

                if (!string.IsNullOrEmpty(estado) && Enum.TryParse<EstadoPedido>(estado, out var estadoEnum))
                {
                    pedidos = pedidos.Where(p => p.Estado == estadoEnum).ToList();
                }

                var desglosePedidos = new Dictionary<int, (decimal subtotal, decimal impuesto, decimal total)>();

                foreach (var pedido in pedidos)
                {
                    var responseDesglose = await httpClient.GetAsync($"api/Pedido/CalcularDesgloseDePedido?idPedido={pedido.Id}");

                    if (responseDesglose.IsSuccessStatusCode)
                    {
                        var jsonDesglose = await responseDesglose.Content.ReadAsStringAsync();
                        var desgloseDto = JsonSerializer.Deserialize<DesglosePedidoDto>(jsonDesglose, JsonOpciones);

                        if (desgloseDto != null)
                        {
                            desglosePedidos[pedido.Id] = (desgloseDto.Subtotal, desgloseDto.Impuesto, desgloseDto.Total);
                        }
                        else
                        {
                            desglosePedidos[pedido.Id] = (0m, 0m, 0m);
                        }
                    }
                    else
                    {
                        desglosePedidos[pedido.Id] = (0m, 0m, 0m);
                    }
                }

                ViewBag.FechaInicial = fechaInicial.ToString("yyyy-MM-dd");
                ViewBag.FechaFinal = fechaFinal.ToString("yyyy-MM-dd");
                ViewBag.EstadoSeleccionado = estado;
                ViewBag.Resultados = pedidos;
                ViewBag.DesglosePedidos = desglosePedidos;
                ViewBag.HuboConsulta = true;

                if (!pedidos.Any())
                {
                    ViewBag.MensajeVacio = "No se encontraron pedidos en el rango de fechas especificado.";
                }

                return View();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "El servicio de la API no está disponible en este momento. Intente más tarde.");
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado: " + ex.Message);
                return View();
            }
        }
    }
}