using GestorDeRestaurante.AccesoADatos.Contexto;
using SistemaAlquilerPlaya.Dominio.Entidades;
using SistemaAlquilerPlaya.AccesoADatos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaAlquilerPlaya.AccesoADatos.Servicios
{
    public class ArticuloRepositorio : IArticuloRepositorio
    {
        private readonly DbContexto _contexto;

        public ArticuloRepositorio(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Articulo>> ObtenerTodos()
        {
            return await _contexto.Articulos.ToListAsync();
        }

        public async Task<List<Articulo>> ObtenerDisponibles()
        {
            return await _contexto.Articulos
                .Where(x => x.Estado == 1)
                .ToListAsync();
        }

        public async Task<Articulo?> ObtenerPorId(int id)
        {
            return await _contexto.Articulos
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Articulo>> Buscar(string nombre)
        {
            return await _contexto.Articulos
                .Where(x => x.Nombre.Contains(nombre))
                .ToListAsync();
        }

        public async Task Agregar(Articulo articulo)
        {
            await _contexto.Articulos.AddAsync(articulo);
            await _contexto.SaveChangesAsync();
        }

        public async Task Editar(Articulo articulo)
        {
            _contexto.Articulos.Update(articulo);
            await _contexto.SaveChangesAsync();
        }

        public async Task CambiarEstado(int id, int estado)
        {
            var articulo = await ObtenerPorId(id);

            if (articulo == null)
                return;

            articulo.Estado = estado;

            await _contexto.SaveChangesAsync();
        }
    }
}