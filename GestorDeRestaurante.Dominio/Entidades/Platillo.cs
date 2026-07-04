using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestorDeRestaurante.Dominio.Entidades
{
    [Table("Platillos")]
    public class Platillo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del platillo es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre del platillo no puede exceder los 100 caracteres")]
        [RegularExpression(@"^[^\s]+(\s+[^\s]+)*$", ErrorMessage = "El nombre del platillo no puede contener solo espacios en blanco")]
        public string Nombre { get; set; } = null!;

        [MaxLength(300, ErrorMessage = "La descripción del platillo no puede exceder los 300 caracteres")]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción del platillo es obligatoria")]
        public string Descripcion { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor que 0")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "El precio de venta no puede tener más de 2 decimales")]
        [Display(Name = "Precio de Venta")]
        [Required(ErrorMessage = "El precio de venta del platillo es obligatorio")]
        public decimal PrecioDeVenta { get; set; }

        [Required(ErrorMessage = "La categoría del platillo es obligatoria")]
        [Display(Name = "Categoría")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "El estado del platillo es obligatorio")]
        public EstadoDePlatilloEnMenu Estado { get; set; }

        [MaxLength(300, ErrorMessage = "Las observaciones del platillo no pueden exceder los 300 caracteres")]
        public string? Observaciones { get; set; }

        public Receta? Receta { get; set; } = null!;
    }
}
