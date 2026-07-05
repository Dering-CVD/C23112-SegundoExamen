using Microsoft.AspNetCore.Mvc;
using SistemaAlquilerPlaya.UI.Models;
using System.Text;
using System.Text.Json;

namespace SistemaAlquilerPlaya.UI.Controllers
{
    public class AlquileresController : Controller
    {
        private readonly HttpClient _http;

        public AlquileresController(HttpClient http)
        {
            _http = http;
        }

        public async Task<IActionResult> Index()
        {
            var respuesta = await _http.GetAsync("api/Alquileres");

            if (!respuesta.IsSuccessStatusCode)
                return View(new List<AlquilerViewModel>());

            var json = await respuesta.Content.ReadAsStringAsync();

            var lista = JsonSerializer.Deserialize<List<AlquilerViewModel>>
            (
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            return View(lista);
        }
        public IActionResult Alquilar(int id)
        {
            var modelo = new AlquilerViewModel();

            modelo.ArticuloId = id;

            modelo.HoraInicio = DateTime.Now;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Alquilar(AlquilerViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var contenido = new StringContent(
                JsonSerializer.Serialize(modelo),
                Encoding.UTF8,
                "application/json");

            var respuesta = await _http.PostAsync(
                "api/Alquileres",
                contenido);

            if (respuesta.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "No fue posible registrar el alquiler.");

            return View(modelo);
        }
        public async Task<IActionResult> Devolver(int articuloId)
        {
            await _http.PutAsync(
                $"api/Alquileres/devolver/{articuloId}",
                null);

            return RedirectToAction(nameof(Index));
        }
    }
}