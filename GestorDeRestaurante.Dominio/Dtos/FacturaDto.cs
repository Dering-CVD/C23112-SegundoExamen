namespace GestorDeRestaurante.Dominio.Dtos;

public class FacturaDto
{
    public int IdFactura { get; set; }
    public DateTime Fecha { get; set; }
    public int IdPedido { get; set; }
    public int Mesa { get; set; }
    public string TipoPedido { get; set; } = null!;

    public List<FacturaDetalleDto> Detalles { get; set; } = null!;

    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
}

public class FacturaDetalleDto
{
    public string NombrePlatillo { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
public class PedidoFacturableDto
{
    public int IdPedido { get; set; }
    public DateTime FechaPedido { get; set; }
    public string Cliente { get; set; } = null!;
    public int Mesa { get; set; }
    public string TipoPedido { get; set; } = null!;
    public decimal Total { get; set; }
}