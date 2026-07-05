using Microsoft.AspNetCore.Mvc;
using SistemaAlquilerPlaya.UI.Models;
using System.Text;
using System.Text.Json;

namespace SistemaAlquilerPlaya.UI.Controllers
{
    public class ArticulosController : Controller
    {
        private readonly HttpClient _http;

        public ArticulosController(HttpClient http)
        {
            _http = http;
        }

        public async Task<IActionResult> Index()
        {
            var respuesta = await _http.GetAsync("api/Articulos");

            if (!respuesta.IsSuccessStatusCode)
                return View(new List<ArticuloViewModel>());

            var json = await respuesta.Content.ReadAsStringAsync();

            var lista = JsonSerializer.Deserialize<List<ArticuloViewModel>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(lista);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ArticuloViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var contenido = new StringContent(
                JsonSerializer.Serialize(modelo),
                Encoding.UTF8,
                "application/json");

            var respuesta = await _http.PostAsync("api/Articulos", contenido);

            if (respuesta.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return View(modelo);
        }
    }
}