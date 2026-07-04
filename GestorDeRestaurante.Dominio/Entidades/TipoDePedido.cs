using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Entidades;

public enum TipoDePedido
{
    [Display(Name = "Mesa")]
    Mesa = 1,
    [Display(Name = "Para llevar")]
    ParaLlevar = 2,
    [Display(Name = "Express")]
    Express = 3
}
