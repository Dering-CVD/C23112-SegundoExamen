using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Entidades;

public enum EstadoDePlatilloEnCocina
{
    [Display(Name = "Pendiente de preparar")]
    Pendiente = 1,
    [Display(Name = "En preparación")]
    EnPreparacion = 2,
    [Display(Name = "Atendido")]
    Atendido = 3
}
