using Microsoft.AspNetCore.Mvc;
using SistemaAlquilerPlaya.Dominio.Entidades;
using SistemaAlquilerPlaya.LogicaDeNegocios.Interfaces;
using SistemaAlquilerPlaya.LogicaDeNegocios.Servicios;

namespace SistemaAlquilerPlaya.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlquileresController : ControllerBase
    {
        private readonly IAlquilerBL _alquilerBL;

        public AlquileresController(IAlquilerBL alquilerBL)
        {
            _alquilerBL = alquilerBL;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var lista = await _alquilerBL.ObtenerTodos();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            var alquiler = await _alquilerBL.ObtenerDetalle(id);

            if (alquiler == null)
                return NotFound();

            return Ok(alquiler);
        }

        [HttpGet("cliente")]
        public async Task<IActionResult> BuscarCliente(string nombre)
        {
            var lista = await _alquilerBL.BuscarCliente(nombre);
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Alquilar([FromBody] Alquiler alquiler)
        {
            await _alquilerBL.Alquilar(alquiler);
            return Ok();
        }

        [HttpPut("devolver/{articuloId}")]
        public async Task<IActionResult> Devolver(int articuloId)
        {
            await _alquilerBL.Devolver(articuloId);
            return Ok();
        }
    }
}
