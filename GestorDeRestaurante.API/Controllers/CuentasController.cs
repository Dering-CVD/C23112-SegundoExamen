using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        public IServicioDeAutenticacion Autenticacion { get; set; }

        public CuentasController(IServicioDeAutenticacion autenticacion)
        {
            Autenticacion = autenticacion;
        }

        [HttpPost("ValidarCredenciales")]
        public async Task<IActionResult> ValidarCredenciales(LoginDto request)
        {
            var resultado = await Autenticacion.ValidarCredencialesDeInicioDeSesionAsync(request);
            return resultado.Exitoso ? Ok(new { resultado.Mensaje, resultado.Rol }) : BadRequest(resultado.Mensaje);
        }

        [HttpPut("CambiarContrasena")]
        public async Task<IActionResult> CambiarContrasena(CambiarContrasenaDto request)
        {
            var resultado = await Autenticacion.CambiarContrasenaAsync(request);
            return resultado.Exitoso ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
        }
    }
}