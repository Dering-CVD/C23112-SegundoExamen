using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturacionController : ControllerBase
    {
        public IServicioDeFacturacion Facturacion { get; set; }

        public FacturacionController(IServicioDeFacturacion facturacion)
        {
            Facturacion = facturacion;
        }

        [HttpGet("ObtenerPedidosDisponiblesParaFacturar")]
        public async Task<List<PedidoFacturableDto>> ObtenerPedidosDisponiblesParaFacturar()
        {
            return await Facturacion.ObtenerPedidosDisponiblesParaFacturarAsync();
        }

        [HttpGet("PrepararFactura")]
        public async Task<FacturaDto?> PrepararFactura(int idPedido)
        {
            return await Facturacion.PrepararFacturaAsync(idPedido);
        }

        [HttpPost("GenerarFactura")]
        public async Task<IActionResult> GenerarFactura(int idPedido)
        {
            try
            {
                var factura = await Facturacion.GenerarFacturaAsync(idPedido);
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}