using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientesController : ControllerBase
    {
        public IServicioDeIngrediente Ingredientes { get; set; }

        public IngredientesController(IServicioDeIngrediente ingredientes)
        {
            Ingredientes = ingredientes;
        }

        [HttpGet("ObtenerIngredientes")]
        public List<Ingrediente> ObtenerIngredientes()
        {
            return Ingredientes.ObtenerIngredientes();
        }

        [HttpGet("BuscarPorNombre")]
        public List<Ingrediente> BuscarPorNombre(string nombre)
        {
            return Ingredientes.BuscarPorNombre(nombre);
        }

        [HttpGet("ObtenerPorId")]
        public Ingrediente ObtenerPorId(int id)
        {
            return Ingredientes.ObtenerPorId(id);
        }

        [HttpPost("Agregar")]
        public IActionResult Agregar(Ingrediente ingrediente)
        {
            try
            {
                Ingredientes.Agregar(ingrediente);
                return Ok(ingrediente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Editar")]
        public IActionResult Editar(Ingrediente ingrediente)
        {
            try
            {
                Ingredientes.Editar(ingrediente);
                return Ok(ingrediente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("AjustarInventario")]
        public IActionResult AjustarInventario(int id, double cantidad, string tipo)
        {
            try
            {
                Ingredientes.AjustarInventario(id, cantidad, tipo);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Activar")]
        public IActionResult Activar(int id)
        {
            try
            {
                Ingredientes.Activar(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Inactivar")]
        public IActionResult Inactivar(int id)
        {
            try
            {
                Ingredientes.Inactivar(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}