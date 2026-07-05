using SistemaAlquilerPlaya.Dominio.Entidades;

namespace SistemaAlquilerPlaya.LogicaDeNegocios.Interfaces
{
    public interface IArticuloBL
    {
        Task<List<Articulo>> ObtenerTodos();

        Task<List<Articulo>> ObtenerDisponibles();

        Task<List<Articulo>> Buscar(string nombre);

        Task Registrar(Articulo articulo);

        Task Editar(Articulo articulo);

        Task CambiarEstado(int articuloId, int estado);
    }
}