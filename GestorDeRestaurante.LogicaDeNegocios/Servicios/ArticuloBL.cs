using SistemaAlquilerPlaya.AccesoADatos.Interfaces;
using SistemaAlquilerPlaya.Dominio.Entidades;
using SistemaAlquilerPlaya.LogicaDeNegocios.Interfaces;

namespace SistemaAlquilerPlaya.LogicaDeNegocios.Servicios
{
    public class ArticuloBL : IArticuloBL
    {
        private readonly IArticuloRepositorio _repositorio;

        public ArticuloBL(IArticuloRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Articulo>> ObtenerTodos()
        {
            return await _repositorio.ObtenerTodos();
        }

        public async Task<List<Articulo>> ObtenerDisponibles()
        {
            return await _repositorio.ObtenerDisponibles();
        }

        public async Task<List<Articulo>> Buscar(string nombre)
        {
            return await _repositorio.Buscar(nombre);
        }

        public async Task Registrar(Articulo articulo)
        {
            if (string.IsNullOrWhiteSpace(articulo.Nombre))
                throw new Exception("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(articulo.Tipo))
                throw new Exception("El tipo es obligatorio.");

            if (string.IsNullOrWhiteSpace(articulo.Descripcion))
                throw new Exception("La descripción es obligatoria.");

            if (articulo.PrecioHora <= 0)
                throw new Exception("El precio debe ser mayor que cero.");

            articulo.Estado = 1;

            await _repositorio.Agregar(articulo);
        }

        public async Task Editar(Articulo articulo)
        {
            if (string.IsNullOrWhiteSpace(articulo.Nombre))
                throw new Exception("El nombre es obligatorio.");

            if (articulo.PrecioHora <= 0)
                throw new Exception("Precio inválido.");

            await _repositorio.Editar(articulo);
        }

        public async Task CambiarEstado(int articuloId, int estado)
        {
            await _repositorio.CambiarEstado(articuloId, estado);
        }
    }
}