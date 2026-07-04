using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        public IServicioDeMenu Menu { get; set; }

        public MenuController(IServicioDeMenu menu)
        {
            Menu = menu;
        }

        [HttpGet("ObtengaLaListaDePlatillos")]
        public List<Platillo> ObtengaLaListaDePlatillos()
        {
            return Menu.ObtengaLaListaDePlatillos();
        }

        [HttpGet("ObtengaLaListaDePlatillosPorNombre")]
        public List<Platillo> ObtengaLaListaDePlatillosPorNombre(string nombre)
        {
            return Menu.ObtengaLaListaDePlatillosPorNombre(nombre);
        }

        [HttpGet("ObtengaElPlatilloPorId")]
        public Platillo ObtengaElPlatilloPorId(int id)
        {
            return Menu.ObtengaElPlatilloPorId(id);
        }

        [HttpPost("AgregarPlatillo")]
        public IActionResult AgregarPlatillo(Platillo platillo)
        {
            try
            {
                Menu.AgregarPlatillo(platillo);
                return Ok(platillo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("EditarPlatillo")]
        public IActionResult EditarPlatillo(Platillo platillo)
        {
            try
            {
                Menu.EditarPlatillo(platillo);
                return Ok(platillo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}