using SistemaAlquilerPlaya.AccesoADatos.Contexto;
using SistemaAlquilerPlaya.Dominio.Entidades;
using SistemaAlquilerPlaya.AccesoADatos.Interfaces;

namespace SistemaAlquilerPlaya.AccesoADatos.Repositorio
{
    public class AlquilerRepositorio : IAlquilerRepositorio
    {
        private readonly DbContexto _contexto;

        public AlquilerRepositorio(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Alquiler>> ObtenerTodos()
        {
            return await _contexto.Alquileres
                .Include(x => x.Articulo)
                .ToListAsync();
        }

        public async Task<List<Alquiler>> BuscarPorCliente(string nombre)
        {
            return await _contexto.Alquileres
                .Include(x => x.Articulo)
                .Where(x => x.NombreCliente.Contains(nombre))
                .ToListAsync();
        }

        public async Task<Alquiler?> ObtenerPorId(int id)
        {
            return await _contexto.Alquileres
                .Include(x => x.Articulo)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Agregar(Alquiler alquiler)
        {
            await _contexto.Alquileres.AddAsync(alquiler);
            await _contexto.SaveChangesAsync();
        }
    }
}