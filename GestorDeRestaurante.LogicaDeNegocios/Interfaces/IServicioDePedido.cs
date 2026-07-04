using GestorDeRestaurante.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces
{
    public interface IServicioDePedido
    {
        List<Pedido> ObtenerLaListaDePedidos();
        List<Pedido> ObtengaLaListaDePedidosPorNombreDelCliente(string nombreDelCliente);
        List<PedidoDetalle> ObtenerDetalleDePedido(int idPedido);
        void CrearPedido(Pedido nuevoPedido);
        void AgregarPlatiloAlPedido(Platillo platillo, Pedido pedido, int cantidad, string? observaciones = null);
        void EliminarPlatilloDelPedido(int idPedidoDetalle);
        void EditarPedido(Pedido pedidoEditado);
        void CancelarPedido(int idPedido, string motivoDeLaCancelacion, bool ignorarAdvertenciaPlatillosAtendidos = false);
        Pedido ObtenerPedidoPorId(int id);
        List<PedidoDetalle> ObtenerDetalleDePedidoConPlatillo(int idPedido);
        decimal CalcularTotalDelPedido(int idPedido);
        List<Pedido> ObtenerPedidosPorRangoDeFechas(DateTime fechaInicial, DateTime fechaFinal);
         List<PedidoDetalle> ObtenerDetalleDePedidoFiltradoPorNombre(int idPedido, string nombrePlatillo);
        List<Platillo> ObtenerPlatillosDisponiblesFiltradoPorNombre(string nombrePlatillo);
        (decimal subtotal, decimal impuesto, decimal total) CalcularDesgloseDePedido(int idPedido, decimal porcentajeImpuesto = 0.13m);
        void ActualizarEstadoDelPedido(int idPedido);
    }
}
