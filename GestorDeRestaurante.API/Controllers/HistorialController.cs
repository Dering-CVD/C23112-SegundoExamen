using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly IServicioDePedido Historial;

        public HistorialController(IServicioDePedido historial)
        {
            Historial = historial;
        }

        [HttpGet("ObtenerHistorial")]
        public IActionResult ObtenerHistorial(DateTime fechaInicial, DateTime fechaFinal, string? estado)
        {
            if (fechaInicial > fechaFinal)
            {
                return BadRequest("La fecha inicial no puede ser posterior a la fecha final.");
            }

            var pedidos = Historial.ObtenerPedidosPorRangoDeFechas(fechaInicial, fechaFinal);

            if (!string.IsNullOrEmpty(estado) && Enum.TryParse<EstadoPedido>(estado, out var estadoEnum))
            {
                pedidos = pedidos.Where(p => p.Estado == estadoEnum).ToList();
            }

            return Ok(pedidos);
        }

        [HttpGet("ObtenerDesglose")]
        public IActionResult ObtenerDesglose(int idPedido)
        {
            var (subtotal, impuesto, total) = Historial.CalcularDesgloseDePedido(idPedido);

            return Ok(new
            {
                Subtotal = subtotal,
                Impuesto = impuesto,
                Total = total
            });
        }
    }
}
