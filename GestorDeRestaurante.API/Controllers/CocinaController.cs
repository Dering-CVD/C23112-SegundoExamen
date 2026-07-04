using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CocinaController : ControllerBase
    {
        public IServicioDeCocina Cocina { get; set; }
        public IServicioDePedido Pedido { get; set; }

        public CocinaController(IServicioDeCocina cocina, IServicioDePedido pedido)
        {
            Cocina = cocina;
            Pedido = pedido;
        }


        [HttpGet("ListarPedidosPendientesAsync")]
        public async Task<IActionResult> ListarPedidosPendientesAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null)
        {
            var pedidosPendientes = await Cocina.ListarPedidosPendientesAsync(filtroNombre, filtrarPedido, filtrarTipo);
            return Ok(pedidosPendientes);
        }

        [HttpGet("ListarPedidosEnPreparacionAsync")]
        public async Task<IActionResult> ListarPedidosEnPreparacion(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null)
        {
            var pedidosEnPreparacion = await Cocina.ListarPedidosEnPreparacionAsync(filtroNombre, filtrarPedido, filtrarTipo);
            return Ok(pedidosEnPreparacion);
        }

        [HttpGet("ListarPedidosAtendidosAsync")]
        public async Task<IActionResult> ListarPedidosAtendidosAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null)
        {
            var pedidosAtendidos = await Cocina.ListarPedidosAtendidosAsync(filtroNombre, filtrarPedido, filtrarTipo);
            return Ok(pedidosAtendidos);
        }

        [HttpGet("ObtenerDetallesRecetaAsync")]
        public async Task<IActionResult> ObtenerDetallesRecetaAsync(int idDetalle)
        {
            var detallesReceta = await Cocina.ObtenerDetallesRecetaAsync(idDetalle);
            if (detallesReceta == null) return NotFound(new { message = "Receta no encontrada" });

            return Ok(detallesReceta);
        }

        [HttpPost("IniciarPreparacionAsync")]
        public async Task<IActionResult> IniciarPreparacionAsync(int idDetalle)
        {
            var resultado = await Cocina.IniciarPreparacionAsync(idDetalle);

            if (!resultado.Exitoso)
            {
                return BadRequest(new { success = false, message = resultado.Mensaje });
            }

            return Ok(new { success = true, message = resultado.Mensaje });
        }

        [HttpPost("MarcarPedidoComoAtendidoAsync")]
        public async Task<IActionResult> MarcarPedidoComoAtendido(int idDetalle, string? observaciones = null)
        {
            var completado = await Cocina.MarcarPedidoComoAtendidoAsync(idDetalle, observaciones);
            if (!completado)
                return BadRequest("No se pudo marcar el platillo como atendido.");
            Pedido.ActualizarEstadoDelPedido(idDetalle);
            return Ok("El platillo ha sido marcado como atendido con éxito.");
        }
    }
}
