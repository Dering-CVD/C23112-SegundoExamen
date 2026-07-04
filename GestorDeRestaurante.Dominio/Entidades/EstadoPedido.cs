using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace GestorDeRestaurante.Dominio.Entidades;

public enum EstadoPedido
{
    [Display(Name = "Pendiente de preparar")]
    Pendiente = 1,
    [Display(Name = "Atendido")]
    Atendido = 2,
    [Display(Name = "Facturado")]
    Facturado = 3,
    [Display(Name = "Cancelado")]
    Cancelado = 4,
    [Display(Name = "En preparación")]
    EnPreparacion = 5
}