using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeRestaurante.Dominio.Entidades;

[Table("Pedido")]
public class Pedido
{
    [Key]
    public int Id { get; set; }

    public EstadoPedido Estado { get; set; }

    [MaxLength(100, ErrorMessage = "El nombre del cliente no puede exceder los 100 caracteres.")]
    [Display(Name = "Nombre del Cliente")]
    public string? NombreDelCliente { get; set; } = null!;

    [Range(1, 10, ErrorMessage = "El número de mesa debe ser un valor positivo entre 1 y 10.")]
    [Display(Name = "Número de Mesa")]
    [Required(ErrorMessage = "El número de mesa es obligatorio")]
    public int NumeroDeMesa { get; set; }

    [Display(Name = "Observaciones")]
    [MaxLength(300, ErrorMessage = "Las observaciones no pueden exceder los 300 caracteres.")]
    public string? Observaciones { get; set; }

    [Display(Name = "Tipo de Pedido")]
    [Required(ErrorMessage = "El tipo de pedido es obligatorio.")]
    public TipoDePedido TipoDePedido { get; set; }

    [Display(Name = "Motivo de la Cancelación")]
    [MinLength(10, ErrorMessage = "El motivo de la cancelación debe tener al menos 10 caracteres.")]
    [MaxLength(500, ErrorMessage = "El motivo de la cancelación no puede exceder los 500 caracteres.")]
    public string? MotivoDeLaCancelacion { get; set; }

    public DateTime FechaDeCreacion { get; set; }
}