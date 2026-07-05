using SistemaAlquilerPlaya.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaAlquilerPlaya.AccesoADatos.Interfaces
{
    public interface IArticuloRepositorio
    {
        Task<List<Articulo>> ObtenerTodos();

        Task<List<Articulo>> ObtenerDisponibles();

        Task<Articulo?> ObtenerPorId(int id);

        Task<List<Articulo>> Buscar(string nombre);

        Task Agregar(Articulo articulo);

        Task Editar(Articulo articulo);

        Task CambiarEstado(int id, int estado);
    }
}