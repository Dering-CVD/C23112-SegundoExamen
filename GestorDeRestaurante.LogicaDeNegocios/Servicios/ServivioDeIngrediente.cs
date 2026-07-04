using System;
using System.Collections.Generic;
using System.Text;
using GestorDeRestaurante.AccesoADatos.Contexto;
using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios;

public class ServicioDeIngrediente : IServicioDeIngrediente
{
    private readonly DbContexto _contexto;

    public ServicioDeIngrediente(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public List<Ingrediente> ObtenerIngredientes()
    {
        return _contexto.Ingredientes.ToList();
    }

    public List<Ingrediente> BuscarPorNombre(string nombre)
    {
        return _contexto.Ingredientes
            .Where(i => i.Nombre.ToLower()
            .Contains(nombre.ToLower()))
            .ToList();
    }

    public Ingrediente ObtenerPorId(int id)
    {
        return _contexto.Ingredientes.FirstOrDefault(i => i.Id == id)!;
    }

    public void Agregar(Ingrediente ingrediente)
    {
        bool existe = _contexto.Ingredientes.Any(i => i.Nombre.ToLower() == ingrediente.Nombre.ToLower() && i.UnidadDeMedida == ingrediente.UnidadDeMedida);

        if (existe)
        {
            throw new Exception("Ya existe un ingrediente con ese nombre");
        }

        ingrediente.CantiadadDisponible = 0;
        ingrediente.Estado = 1;

        _contexto.Ingredientes.Add(ingrediente);

        _contexto.SaveChanges();
    }

    public void Editar(Ingrediente ingrediente)
    {
        var ingredienteBD = ObtenerPorId(ingrediente.Id);

        if (ingredienteBD == null)
        {
            throw new Exception("Ingrediente no encontrado");
        }

        ingredienteBD.Nombre = ingrediente.Nombre;
        ingredienteBD.UnidadDeMedida = ingrediente.UnidadDeMedida;
        ingredienteBD.CostoUnitario = ingrediente.CostoUnitario;
        ingredienteBD.CantidadMinima = ingrediente.CantidadMinima;

        _contexto.SaveChanges();
    }

    public void AjustarInventario(int id, double cantidad, string tipo)
    {
        var ingrediente = ObtenerPorId(id);

        if (ingrediente == null)
        {
            throw new Exception("Ingrediente no encontrado");
        }

        if (tipo == "Aumento")
        {
            ingrediente.CantiadadDisponible += cantidad;
        }
        else
        {
            if (ingrediente.CantiadadDisponible < cantidad)
            {
                throw new Exception("No hay suficiente inventario");
            }

            ingrediente.CantiadadDisponible -= cantidad;
        }

        _contexto.SaveChanges();
    }

    public void Activar(int id)
    {
        var ingrediente = ObtenerPorId(id);

        ingrediente.Estado = 1;

        _contexto.SaveChanges();
    }

    public void Inactivar(int id)
    {
        var ingrediente = ObtenerPorId(id);

        ingrediente.Estado = 0;

        _contexto.SaveChanges();
    }
}