using System.ComponentModel.DataAnnotations;

namespace GestorDeRestaurante.Dominio.Entidades
{
    public enum Categoria
    {
        Entrada = 1,
        [Display(Name = "Plato Fuerte")]
        PlatoFuerte = 2,
        Bebida = 3,
        Postre = 4,
        Combo = 5,
        Otros = 6
    }
}