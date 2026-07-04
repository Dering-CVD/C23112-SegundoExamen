using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeRestaurante.Dominio.Entidades;

[Table("PedidoDetalle")]
public class PedidoDetalle
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Pedido))]
    public int IdPedido { get; set; }

    [ForeignKey(nameof(Platillo))]
    public int IdPlatillos { get; set; }

    [Display(Name = "Observaciones")]
    [MaxLength(300)]
    public string? Observaciones { get; set; }

    public EstadoDePlatilloEnCocina Estado { get; set; }

    [Display(Name = "Fecha de inicio de preparación")]
    public DateTime? FechaDeInicioPreparacion { get; set; }

    [Display(Name = "Observaciones de cocina")]
    [Column("ObservacionesCocina")]
    public string? ObservacionesCocina { get; set; }

    [Display(Name = "Fecha final de preparación")]
    public DateTime? FechaFinalDePreparacion { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    [ForeignKey("IdPedido")]
    public Pedido? Pedido { get; set; } = null!;

    [ForeignKey("IdPlatillos")]
    public Platillo? Platillo { get; set; } = null!;
}