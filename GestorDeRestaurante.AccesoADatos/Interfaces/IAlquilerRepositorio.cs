using SistemaAlquilerPlaya.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaAlquilerPlaya.AccesoADatos.Interfaces
{
    public interface IAlquilerRepositorio
    {
        Task<List<Alquiler>> ObtenerTodos();

        Task<List<Alquiler>> BuscarPorCliente(string nombre);

        Task<Alquiler?> ObtenerPorId(int id);

        Task Agregar(Alquiler alquiler);
    }
}