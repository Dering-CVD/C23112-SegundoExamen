using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Entidades;

public class Ingrediente
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del ingrediente es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre del ingrediente no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = null!;
    public double CantiadadDisponible { get; set; }
    public UnidadDeMedida UnidadDeMedida { get; set; }
    [Required(ErrorMessage = "La cantidad mínima es obligatorio")]
    public double CantidadMinima { get; set; }
    public int Estado { get; set; }
    [Range(0, 999999.99, ErrorMessage = "El costo unitario no puede ser negativo.")]
    [Required(ErrorMessage = "El costo unitario es obligatorio")]
    public decimal CostoUnitario { get; set; }
}