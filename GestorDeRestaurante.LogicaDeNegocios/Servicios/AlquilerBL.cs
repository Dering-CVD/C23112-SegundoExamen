using SistemaAlquilerPlaya.AccesoADatos.Interfaces;
using SistemaAlquilerPlaya.Dominio.Entidades;
using SistemaAlquilerPlaya.LogicaDeNegocios.Interfaces;

namespace SistemaAlquilerPlaya.LogicaDeNegocios.Servicios
{
    public class AlquilerBL : IAlquilerBL
    {
        private readonly IAlquilerRepositorio _alquilerRepo;
        private readonly IArticuloRepositorio _articuloRepo;

        public AlquilerBL(
            IAlquilerRepositorio alquilerRepo,
            IArticuloRepositorio articuloRepo)
        {
            _alquilerRepo = alquilerRepo;
            _articuloRepo = articuloRepo;
        }

        public async Task<List<Alquiler>> ObtenerTodos()
        {
            return await _alquilerRepo.ObtenerTodos();
        }

        public async Task<List<Alquiler>> BuscarCliente(string nombre)
        {
            return await _alquilerRepo.BuscarPorCliente(nombre);
        }

        public async Task<Alquiler?> ObtenerDetalle(int id)
        {
            return await _alquilerRepo.ObtenerPorId(id);
        }

        public decimal CalcularMonto(decimal precioHora, int horas)
        {
            decimal monto = precioHora * horas;

            if (horas >= 3 && horas <= 5)
                monto *= 0.90m;

            else if (horas > 5)
                monto *= 0.50m;

            return monto;
        }

        public async Task Alquilar(Alquiler alquiler)
        {
            if (string.IsNullOrWhiteSpace(alquiler.IdentificacionCliente))
                throw new Exception("Identificación requerida.");

            if (string.IsNullOrWhiteSpace(alquiler.NombreCliente))
                throw new Exception("Nombre requerido.");

            if (alquiler.CantidadHoras <= 0)
                throw new Exception("Las horas deben ser mayores que cero.");

            var articulo = await _articuloRepo.ObtenerPorId(alquiler.ArticuloId);

            if (articulo == null)
                throw new Exception("Artículo inexistente.");

            if (articulo.Estado == 2)
                throw new Exception("El artículo ya está alquilado.");

            alquiler.FechaAlquiler = DateTime.Today;

            alquiler.MontoTotal = CalcularMonto(
                articulo.PrecioHora,
                alquiler.CantidadHoras);

            await _alquilerRepo.Agregar(alquiler);

            await _articuloRepo.CambiarEstado(alquiler.ArticuloId, 2);
        }

        public async Task Devolver(int articuloId)
        {
            await _articuloRepo.CambiarEstado(articuloId, 1);
        }
    }
}