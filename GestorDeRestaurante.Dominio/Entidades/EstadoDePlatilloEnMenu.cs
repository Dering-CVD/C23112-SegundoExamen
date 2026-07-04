using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Entidades
{
    public enum EstadoDePlatilloEnMenu
    {
        Disponible = 1,
        [Display(Name = "No Disponible")]
        NoDisponible = 2,
    }
}