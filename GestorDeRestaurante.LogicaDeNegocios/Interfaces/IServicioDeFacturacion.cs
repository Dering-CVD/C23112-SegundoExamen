using GestorDeRestaurante.Dominio.Dtos;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces;

public interface IServicioDeFacturacion
{
    Task<List<PedidoFacturableDto>> ObtenerPedidosDisponiblesParaFacturarAsync();
    Task<FacturaDto?> PrepararFacturaAsync(int idPedido);
    Task<FacturaDto> GenerarFacturaAsync(int idPedido);
}