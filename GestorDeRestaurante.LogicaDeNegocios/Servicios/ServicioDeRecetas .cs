using System;
using System.Collections.Generic;
using System.Text;
using GestorDeRestaurante.AccesoADatos.Contexto;
using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios
{
    public class ServicioDeRecetas : IServicioDeRecetas
    {
        private readonly DbContexto contexto;

        public ServicioDeRecetas(DbContexto contexto)
        {
            this.contexto = contexto;
        }

        public List<Receta> ObtengaLaListaDeRecetas()
        {
            return contexto.Receta
                .Include(r => r.Platillo)
                .ToList();
        }

        public List<Receta> ObtengaLaListaDeRecetasPorPlatillo(string nombre)
        {
            return contexto.Receta
                .Include(r => r.Platillo)
                .Where(r => r.Platillo.Nombre.Contains(nombre))
                .ToList();
        }

        public Receta ObtengaLaRecetaPorId(int id)
        {
            return contexto.Receta
                .Include(r => r.Platillo)
                .Include(r => r.RecetaIngredientes)
                .ThenInclude(ri => ri.Ingrediente)
                .FirstOrDefault(r => r.Id == id)!;
        }

        public void AgregarReceta(Receta receta)
        {
            bool existeReceta = contexto.Receta
                .Any(r => r.IdPlatillos == receta.IdPlatillos);

            if (existeReceta)
            {
                throw new Exception("El platillo ya tiene una receta registrada.");
            }

            contexto.Receta.Add(receta);
            contexto.SaveChanges();
        }

        public void AgregarIngredienteAReceta(RecetaDetalleIngrediente recetaIngrediente)
        {
            recetaIngrediente.Obsvacion ??= string.Empty;

            bool ingredienteDuplicado = contexto.RecetaDetalleIngredientes
                .Any(ri =>
                    ri.IdReceta == recetaIngrediente.IdReceta &&
                    ri.IdIngrediente == recetaIngrediente.IdIngrediente);

            if (ingredienteDuplicado)
            {
                throw new Exception("El ingrediente ya existe en la receta.");
            }

            if (recetaIngrediente.Cantidad <= 0)
            {
                throw new Exception("La cantidad requerida debe ser mayor a 0.");
            }

            contexto.RecetaDetalleIngredientes.Add(recetaIngrediente);
            contexto.SaveChanges();
        }

        public void EliminarIngredienteDeReceta(int id)
        {
            RecetaDetalleIngrediente recetaIngrediente = contexto.RecetaDetalleIngredientes.FirstOrDefault(ri => ri.Id == id)!;

            if (recetaIngrediente != null)
            {
                contexto.RecetaDetalleIngredientes.Remove(recetaIngrediente);
                contexto.SaveChanges();
            }
        }

        public List<Ingrediente> ObtengaLaListaDeIngredientes()
        {
            return contexto.Ingredientes
                .Where(i => i.Estado == 1)
                .ToList();
        }

        public List<RecetaDetalleIngrediente> ObtengaIngredientesDeReceta(int idReceta)
        {
            return contexto.RecetaDetalleIngredientes
                .Include(ri => ri.Ingrediente)
                .Where(ri => ri.IdReceta == idReceta)
                .ToList();
        }
    }
}