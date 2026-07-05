using System.ComponentModel.DataAnnotations;

namespace SistemaAlquilerPlaya.UI.Models
{
    public class AlquilerViewModel
    {
        public int Id { get; set; }

        public int ArticuloId { get; set; }

        public string NombreArticulo { get; set; } = "";

        [Required]
        public string IdentificacionCliente { get; set; } = "";

        [Required]
        public string NombreCliente { get; set; } = "";

        [Required]
        public DateTime HoraInicio { get; set; }

        [Required]
        [Range(1, 24)]
        public int CantidadHoras { get; set; }

        public decimal PrecioHora { get; set; }

        public decimal MontoTotal { get; set; }
    }
}