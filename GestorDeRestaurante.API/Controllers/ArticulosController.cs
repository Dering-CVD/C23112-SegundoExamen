using Microsoft.AspNetCore.Mvc;
using SistemaAlquilerPlaya.Dominio.Entidades;
using SistemaAlquilerPlaya.LogicaDeNegocios.Interfaces;

namespace SistemaAlquilerPlaya.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosController : ControllerBase
    {
        private readonly IArticuloBL _articuloBL;

        public ArticulosController(IArticuloBL articuloBL)
        {
            _articuloBL = articuloBL;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var lista = await _articuloBL.ObtenerTodos();
            return Ok(lista);
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            var lista = await _articuloBL.ObtenerDisponibles();
            return Ok(lista);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar(string nombre)
        {
            var lista = await _articuloBL.Buscar(nombre);
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Articulo articulo)
        {
            try
            {
                await _articuloBL.Registrar(articulo);
                return Ok();
            }
                catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Articulo articulo)
        {
            await _articuloBL.Editar(articulo);
            return Ok();
        }
    }
}