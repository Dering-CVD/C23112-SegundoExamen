using System;
using System.Collections.Generic;
using System.Text;
using GestorDeRestaurante.Dominio.Entidades;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces;

public interface IServicioDeIngrediente
{
    List<Ingrediente> ObtenerIngredientes();

    List<Ingrediente> BuscarPorNombre(string nombre);

    Ingrediente ObtenerPorId(int id);

    void Agregar(Ingrediente ingrediente);

    void Editar(Ingrediente ingrediente);

    void AjustarInventario(int id, double cantidad, string tipo);

    void Activar(int id);

    void Inactivar(int id);
}