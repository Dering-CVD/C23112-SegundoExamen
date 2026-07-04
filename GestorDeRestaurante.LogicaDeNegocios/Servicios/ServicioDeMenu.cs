using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios
{
    public class ServicioDeMenu : IServicioDeMenu
    {
        private GestorDeRestaurante.AccesoADatos.Contexto.DbContexto ContextoDeDatos;

        public ServicioDeMenu(GestorDeRestaurante.AccesoADatos.Contexto.DbContexto dbContext)
        {
            ContextoDeDatos = dbContext;
        }
        public List<Platillo> ObtengaLaListaDePlatillos()
        {
            var resultado = from c in ContextoDeDatos.Platillos
                            select c;
            return resultado.ToList();
        }
        public List<Platillo> ObtengaLaListaDePlatillosPorNombre(string nombre)
        {
            var resultado = from c in ContextoDeDatos.Platillos
                            where c.Nombre.ToLower().Contains(nombre.ToLower())
                            select c;
            return resultado.ToList();
        }
        public Platillo ObtengaElPlatilloPorId(int id)
        {
            Platillo platillo;
            platillo = ContextoDeDatos.Platillos.Find(id)!;
            return platillo;
        }
        public void AgregarPlatillo(Platillo nuevoPlatillo)
        {
            if (string.IsNullOrWhiteSpace(nuevoPlatillo.Nombre))
            {
                throw new Exception("El nombre del platillo es un campo obligatorio y no puede contener solo espacios en blanco.");
            }

            if (string.IsNullOrWhiteSpace(nuevoPlatillo.Descripcion))
            {
                throw new Exception("La descripción del platillo es un campo obligatorio y no puede contener solo espacios en blanco.");
            }

            bool existeNombre = ContextoDeDatos.Platillos.Any(p =>
                p.Nombre.ToLower().Trim() == nuevoPlatillo.Nombre.ToLower().Trim());

            if (existeNombre)
            {
                throw new Exception($"Ya existe un platillo registrado con el nombre '{nuevoPlatillo.Nombre.Trim()}'. El nombre debe ser único.");
            }

            if (nuevoPlatillo.PrecioDeVenta <= 0)
            {
                throw new Exception("El precio de venta debe ser un valor numérico mayor que cero.");
            }

            if (nuevoPlatillo.PrecioDeVenta * 100 != Math.Floor(nuevoPlatillo.PrecioDeVenta * 100))
            {
                throw new Exception("El precio de venta no puede tener más de dos decimales.");
            }

            nuevoPlatillo.Nombre = nuevoPlatillo.Nombre.Trim();
            nuevoPlatillo.Descripcion = nuevoPlatillo.Descripcion.Trim();
            nuevoPlatillo.Observaciones = nuevoPlatillo.Observaciones?.Trim();
            nuevoPlatillo.Estado = EstadoDePlatilloEnMenu.Disponible;
            ContextoDeDatos.Platillos.Add(nuevoPlatillo);
            ContextoDeDatos.SaveChanges();
        }

        public void EditarPlatillo(Platillo platillo)
        {
            Platillo platilloAEditar = ObtengaElPlatilloPorId(platillo.Id);

            platilloAEditar.Nombre = platillo.Nombre.Trim();
            platilloAEditar.Descripcion = platillo.Descripcion.Trim();
            platilloAEditar.Categoria = platillo.Categoria;
            platilloAEditar.PrecioDeVenta = platillo.PrecioDeVenta;
            platilloAEditar.Estado = platillo.Estado;
            ContextoDeDatos.SaveChanges();
        }
        
    }
}
