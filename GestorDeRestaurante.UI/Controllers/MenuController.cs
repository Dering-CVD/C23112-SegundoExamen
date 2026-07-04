using GestorDeRestaurante.Dominio.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace GestorDeRestaurante.UI.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    public class MenuController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        private static readonly JsonSerializerOptions JsonOpciones = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public MenuController(IConfiguration configuration)
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
                    response = await httpClient.GetAsync("api/Menu/ObtengaLaListaDePlatillos");
                else
                    response = await httpClient.GetAsync($"api/Menu/ObtengaLaListaDePlatillosPorNombre?nombre={Uri.EscapeDataString(nombre)}");

                if (!await ValidarRespuestaAsync(response))
                    return View(new List<Platillo>());

                var json = await response.Content.ReadAsStringAsync();
                var platillos = JsonSerializer.Deserialize<List<Platillo>>(json, JsonOpciones);
                return View(platillos ?? new List<Platillo>());
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(new List<Platillo>());
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Platillo platillo)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var json = JsonSerializer.Serialize(platillo);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Menu/AgregarPlatillo", content);

                if (!await ValidarRespuestaAsync(response))
                    return View(platillo);

                TempData["SuccessMessage"] = "Platillo agregado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(platillo);
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var response = await httpClient.GetAsync($"api/Menu/ObtengaElPlatilloPorId?id={id}");

                if (!await ValidarRespuestaAsync(response))
                    return View();

                var json = await response.Content.ReadAsStringAsync();
                var platillo = JsonSerializer.Deserialize<Platillo>(json, JsonOpciones);
                return View(platillo);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(Platillo platillo)
        {
            try
            {
                var httpClient = CrearHttpClient();
                var json = JsonSerializer.Serialize(platillo);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync("api/Menu/EditarPlatillo", content);

                if (!await ValidarRespuestaAsync(response))
                    return View(platillo);

                TempData["SuccessMessage"] = "Platillo actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "El servicio no está disponible. Intente más tarde.");
                return View(platillo);
            }
        }
    }
}