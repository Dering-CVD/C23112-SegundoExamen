using System.ComponentModel.DataAnnotations;

namespace SistemaAlquilerPlaya.UI.Models
{
    public class ArticuloViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Tipo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public decimal PrecioHora { get; set; }

        public int Estado { get; set; }
    }
}