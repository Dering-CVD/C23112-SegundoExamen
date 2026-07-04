using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Entidades;

public class Factura
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime FechaEmision { get; set; }

    [Required]
    public int IdPedido { get; set; }
    public Pedido? Pedido { get; set; } = null!;

    [Required]
    public decimal Subtotal { get; set; }

    [Required]
    public decimal Impuesto { get; set; }

    [Required]
    public decimal Total { get; set; }

}