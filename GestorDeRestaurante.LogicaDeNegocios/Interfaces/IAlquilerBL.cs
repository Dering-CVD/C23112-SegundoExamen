using SistemaAlquilerPlaya.Dominio.Entidades;

namespace SistemaAlquilerPlaya.LogicaDeNegocios.Interfaces
{
    public interface IAlquilerBL
    {
        Task<List<Alquiler>> ObtenerTodos();

        Task<List<Alquiler>> BuscarCliente(string nombre);

        Task<Alquiler?> ObtenerDetalle(int id);

        Task Alquilar(Alquiler alquiler);

        Task Devolver(int articuloId);

        decimal CalcularMonto(decimal precioHora, int horas);
    }
}