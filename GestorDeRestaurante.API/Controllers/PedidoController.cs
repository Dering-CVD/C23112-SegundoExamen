using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace GestorDeRestaurante.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        public IServicioDePedido Pedido { get; set; }
        public IServicioDeMenu Menu { get; set; }
        public PedidoController(IServicioDePedido pedido, IServicioDeMenu menu)
        {
            Pedido = pedido;
            Menu = menu;
        }
        [HttpGet("ObtenerLaListaDePedidos")]
        public List<Pedido> ObtenerLaListaDePedidos()
        {
            return Pedido.ObtenerLaListaDePedidos();
        }

        [HttpGet("ObtengaLaListaDePedidosPorNombreDelCliente")]
        public List<Pedido> ObtengaLaListaDePedidosPorNombreDelCliente(string nombreDelCliente)
        {
            return Pedido.ObtengaLaListaDePedidosPorNombreDelCliente(nombreDelCliente);
        }

        [HttpGet("ObtenerPedidoPorId")]
        public Pedido ObtenerPedidoPorId(int id)
        {
            return Pedido.ObtenerPedidoPorId(id);
        }

        [HttpGet("ObtenerDetalleDePedidoFiltradoPorNombre")]
        public List<PedidoDetalle> ObtenerDetalleDePedidoFiltradoPorNombre(int idPedido, string? nombrePlatillo)
        {
            return Pedido.ObtenerDetalleDePedidoFiltradoPorNombre(idPedido, nombrePlatillo);
        }

        [HttpGet("ObtenerPlatillosDisponiblesFiltradoPorNombre")]
        public List<Platillo> ObtenerPlatillosDisponiblesFiltradoPorNombre(string? nombrePlatillo)
        {
            return Pedido.ObtenerPlatillosDisponiblesFiltradoPorNombre(nombrePlatillo ?? "");
        }

        [HttpGet("CalcularDesgloseDePedido")]
        public IActionResult CalcularDesgloseDePedido(int idPedido, decimal porcentajeImpuesto = 0.13m)
        {
            var (subtotal, impuesto, total) = Pedido.CalcularDesgloseDePedido(idPedido, porcentajeImpuesto);

            return Ok(new
            {
                Subtotal = subtotal,
                Impuesto = impuesto,
                Total = total
            });
        }

        [HttpPost("CrearPedido")]
        public IActionResult CrearPedido(Pedido pedido)
        {
            Pedido.CrearPedido(pedido);
            return Ok(pedido);
        }

        [HttpPost("AgregarPlatilloAlPedido")]
        public IActionResult AgregarPlatilloAlPedido(int idPlatillo, int idPedido, int cantidad, string? observaciones = null)
        {
            var pedido = Pedido.ObtenerPedidoPorId(idPedido);
            var platillo = Menu.ObtengaElPlatilloPorId(idPlatillo);
            if (pedido == null || platillo == null)
            {
                return BadRequest(new { mensaje = "El pedido o el platillo especificado no existe." });
            }
            Pedido.AgregarPlatiloAlPedido(platillo, pedido, cantidad, observaciones);

            return Ok(new { mensaje = "Platillo agregado exitosamente al pedido." });
        }

        [HttpDelete("EliminarPlatilloDelPedido")]
        public IActionResult EliminarPlatilloDelPedido(int idPedidoDetalle)
        {
            Pedido.EliminarPlatilloDelPedido(idPedidoDetalle);
            return Ok(new { mensaje = "El platillo fue eliminado del pedido exitosamente." });
        }
        [HttpPut("EditarPedido")]
        public IActionResult EditarPedido(Pedido pedidoEditado)
        {
            Pedido.EditarPedido(pedidoEditado);
            return Ok(pedidoEditado);
        }
        [HttpPut("CancelarPedido")]
        public IActionResult CancelarPedido(int idPedido, string motivoDeLaCancelacion, bool ignorarAdvertenciaPlatillosAtendidos = false)
        {
            Pedido.CancelarPedido(idPedido, motivoDeLaCancelacion, ignorarAdvertenciaPlatillosAtendidos);
            return Ok(new { mensaje = "El pedido ha sido cancelado exitosamente." });

        }

        [HttpGet("ObtenerPedidosPorRangoDeFechas")]
        public List<Pedido> ObtenerPedidosPorRangoDeFechas(DateTime fechaInicial, DateTime fechaFinal)
        {
            return Pedido.ObtenerPedidosPorRangoDeFechas(fechaInicial, fechaFinal);
        }
        [HttpGet("ActualizarEstadoDePedido")]
        public IActionResult ActualizarEstadoDelPedido(int idPedidoDetalle)
        {
            Pedido.ActualizarEstadoDelPedido(idPedidoDetalle);
            return Ok(new { mensaje = "El estado del pedido ha sido actualizado exitosamente." });
        }
    }
}
