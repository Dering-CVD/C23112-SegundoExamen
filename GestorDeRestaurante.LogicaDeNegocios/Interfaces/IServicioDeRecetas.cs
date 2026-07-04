using System;
using System.Collections.Generic;
using System.Text;
using GestorDeRestaurante.Dominio.Entidades;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces
{
    public interface IServicioDeRecetas
    {
        List<Receta> ObtengaLaListaDeRecetas();

        List<Receta> ObtengaLaListaDeRecetasPorPlatillo(string nombre);

        Receta ObtengaLaRecetaPorId(int id);

        void AgregarReceta(Receta receta);

        void AgregarIngredienteAReceta(RecetaDetalleIngrediente recetaIngrediente);

        void EliminarIngredienteDeReceta(int id);

        List<Ingrediente> ObtengaLaListaDeIngredientes();

        List<RecetaDetalleIngrediente> ObtengaIngredientesDeReceta(int idReceta);
    }
}