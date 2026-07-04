using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Entidades;

public class FacturaDetalle
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int IdFactura { get; set; }
    public Factura? Factura { get; set; } = null!;

    [Required]
    public int IdPlatillo { get; set; }
    public Platillo? Platillo { get; set; } = null!;

    [Required]
    public int Cantidad { get; set; }

    [Required]
    public decimal PrecioUnitario { get; set; }

    [Required]
    public decimal Subtotal { get; set; }
}