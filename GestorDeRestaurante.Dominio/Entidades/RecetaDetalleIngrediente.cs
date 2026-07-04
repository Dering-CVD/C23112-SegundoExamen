using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeRestaurante.Dominio.Entidades;

public class RecetaDetalleIngrediente
{
    [Key]
    public int Id { get; set; }

    [Column("IdReceta")]
    public int IdReceta { get; set; }

    [ForeignKey("IdReceta")]
    public Receta? Receta { get; set; } = null!;

    [Required(ErrorMessage = "El ingrediente es obligatorio.")]
    [Column("IdIngredientes")]
    public int IdIngrediente { get; set; }

    [ForeignKey("IdIngrediente")]
    public Ingrediente? Ingrediente { get; set; } = null!;

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, 10000, ErrorMessage = "La cantidad debe ser un valor mayor a 0.")]
    public int Cantidad { get; set; }

    [Column("Obsvacion")]
    [MaxLength(200, ErrorMessage = "La observación no puede exceder los 200 caracteres.")]
    public string? Obsvacion { get; set; } = string.Empty;
}