using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeRestaurante.Dominio.Entidades;

public class Receta
{
    [Key]
    public int Id { get; set; }

    [Column("IdPlatillos")]
    [Required(ErrorMessage = "Debe seleccionar un platillo")]
    public int IdPlatillos { get; set; }
    [ForeignKey("IdPlatillos")]
    public Platillo? Platillo { get; set; }
    [Required(ErrorMessage = "El nombre de la receta es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre de la receta no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; } = null!;
    [Required(ErrorMessage = "Las porciones es obligatorio")]
    [Range(1, 1000, ErrorMessage = "Las porciones deben ser un valor mayor a 0.")]
    public int PorcionesQueProduce { get; set; }
    public DateTime FechaDeCreacion { get; set; }
    public ICollection<RecetaDetalleIngrediente> RecetaIngredientes { get; set; } = new List<RecetaDetalleIngrediente>();
}
