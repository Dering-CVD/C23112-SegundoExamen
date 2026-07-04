using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecetasController : ControllerBase
    {
        public IServicioDeRecetas Recetas { get; set; }

        public RecetasController(IServicioDeRecetas recetas)
        {
            Recetas = recetas;
        }

        [HttpGet("ObtengaLaListaDeRecetas")]
        public List<Receta> ObtengaLaListaDeRecetas()
        {
            return Recetas.ObtengaLaListaDeRecetas();
        }

        [HttpGet("ObtengaLaListaDeRecetasPorPlatillo")]
        public List<Receta> ObtengaLaListaDeRecetasPorPlatillo(string nombre)
        {
            return Recetas.ObtengaLaListaDeRecetasPorPlatillo(nombre);
        }

        [HttpGet("ObtengaLaRecetaPorId")]
        public Receta ObtengaLaRecetaPorId(int id)
        {
            return Recetas.ObtengaLaRecetaPorId(id);
        }

        [HttpPost("AgregarReceta")]
        public IActionResult AgregarReceta(Receta receta)
        {
            try
            {
                Recetas.AgregarReceta(receta);
                return Ok(receta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtengaLaListaDeIngredientes")]
        public List<Ingrediente> ObtengaLaListaDeIngredientes()
        {
            return Recetas.ObtengaLaListaDeIngredientes();
        }

        [HttpGet("ObtengaIngredientesDeReceta")]
        public List<RecetaDetalleIngrediente> ObtengaIngredientesDeReceta(int idReceta)
        {
            return Recetas.ObtengaIngredientesDeReceta(idReceta);
        }

        [HttpPost("AgregarIngredienteAReceta")]
        public IActionResult AgregarIngredienteAReceta(RecetaDetalleIngrediente recetaIngrediente)
        {
            try
            {
                Recetas.AgregarIngredienteAReceta(recetaIngrediente);
                return Ok(recetaIngrediente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("EliminarIngredienteDeReceta")]
        public IActionResult EliminarIngredienteDeReceta(int id)
        {
            try
            {
                Recetas.EliminarIngredienteDeReceta(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}