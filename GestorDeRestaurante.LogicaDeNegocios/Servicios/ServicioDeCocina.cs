using GestorDeRestaurante.AccesoADatos.Contexto;
using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GestorDeRestaurante.Dominio.Dtos.CocinaDto;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios;

public class ServicioDeCocina : IServicioDeCocina
{
    private readonly DbContexto _contexto;

    public ServicioDeCocina(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<List<ListarPedidosPendientesDto>> ListarPedidosPendientesAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null)
    {
        var consulta = _contexto.PedidoDetalle
            .Where(d => d.Estado == EstadoDePlatilloEnCocina.Pendiente && d.Pedido.Estado != EstadoPedido.Cancelado);

        if (!string.IsNullOrEmpty(filtroNombre))
        {
            consulta = consulta.Where(d => d.Platillo.Nombre.Contains(filtroNombre));
        }

        if (filtrarPedido.HasValue)
        {
            consulta = consulta.Where(d => d.IdPedido == filtrarPedido.Value);
        }

        if (!string.IsNullOrEmpty(filtrarTipo))
        {
            if (Enum.TryParse<TipoDePedido>(filtrarTipo, true, out var tipoEnum))
            {
                consulta = consulta.Where(d => d.Pedido.TipoDePedido == tipoEnum);
            }
        }

        return await consulta.Select(d => new ListarPedidosPendientesDto
        {
            Id = d.Id,
            IdPedido = d.IdPedido,
            FechaDeRegistro = d.Pedido.FechaDeCreacion,
            NombreDelPlatillo = d.Platillo.Nombre,
            Cantidad = d.Cantidad,
            ObservacionDelPlatillo = d.Observaciones!,
            TipoDePedido = d.Pedido.TipoDePedido,
            NombreDelCliente = d.Pedido.NombreDelCliente,
            ObservacionesPedido = d.Pedido.Observaciones ?? string.Empty,
            NumeroDeMesa = d.Pedido.NumeroDeMesa
        }).ToListAsync();
    }

    public async Task<List<ListarPedidosEnPreparacionDto>> ListarPedidosEnPreparacionAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null)
    {
        var consulta = _contexto.PedidoDetalle
            .Where(d => d.Estado == EstadoDePlatilloEnCocina.EnPreparacion && d.Pedido.Estado != EstadoPedido.Cancelado);

        if (!string.IsNullOrEmpty(filtroNombre))
        {
            consulta = consulta.Where(d => d.Platillo.Nombre.Contains(filtroNombre));
        }

        if (filtrarPedido.HasValue)
        {
            consulta = consulta.Where(d => d.IdPedido == filtrarPedido.Value);
        }

        if (!string.IsNullOrEmpty(filtrarTipo))
        {
            if (Enum.TryParse<TipoDePedido>(filtrarTipo, true, out var tipoEnum))
            {
                consulta = consulta.Where(d => d.Pedido.TipoDePedido == tipoEnum);
            }
        }

        return await consulta.Select(d => new ListarPedidosEnPreparacionDto
        {
            Id = d.Id,
            IdPedido = d.IdPedido,
            NombreDelPlatillo = d.Platillo.Nombre,
            Cantidad = d.Cantidad,
            FechaDeInicio = d.FechaDeInicioPreparacion ?? DateTime.Now,
            TiempoTranscurrido = DateTime.Now - (d.FechaDeInicioPreparacion ?? DateTime.Now),
            Observaciones = d.Observaciones,
            NombreDelCliente = d.Pedido.NombreDelCliente,
            ObservacionesPedido = d.Pedido.Observaciones ?? string.Empty,
            TipoDePedido = d.Pedido.TipoDePedido,
            NumeroDeMesa = d.Pedido.NumeroDeMesa
        }).ToListAsync();
    }

    public async Task<List<ListarPedidosAtendidosDto>> ListarPedidosAtendidosAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null)
    {
        var hoy = DateTime.Today;
        var consulta = _contexto.PedidoDetalle
            .Where(d => d.Estado == EstadoDePlatilloEnCocina.Atendido && d.FechaFinalDePreparacion >= hoy);

        if (!string.IsNullOrEmpty(filtroNombre))
        {
            consulta = consulta.Where(d => d.Platillo.Nombre.Contains(filtroNombre));
        }

        if (filtrarPedido.HasValue)
        {
            consulta = consulta.Where(d => d.IdPedido == filtrarPedido.Value);
        }

        if (!string.IsNullOrEmpty(filtrarTipo))
        {
            if (Enum.TryParse<TipoDePedido>(filtrarTipo, true, out var tipoEnum))
            {
                consulta = consulta.Where(d => d.Pedido.TipoDePedido == tipoEnum);
            }
        }

        return await consulta.Select(d => new ListarPedidosAtendidosDto
        {
            Id = d.Id,
            IdPedido = d.IdPedido,
            NombreDelPlatillo = d.Platillo.Nombre,
            Cantidad = d.Cantidad,
            FechaDeAtencion = d.FechaFinalDePreparacion ?? DateTime.Now,
            NombreDelCliente = d.Pedido.NombreDelCliente,
            TipoDePedido = d.Pedido.TipoDePedido
        }).ToListAsync();
    }

    public async Task<DetalleRecetaDto?> ObtenerDetallesRecetaAsync(int idDetalle)
    {
        var detalle = await _contexto.PedidoDetalle
            .Include(d => d.Platillo)
            .FirstOrDefaultAsync(d => d.Id == idDetalle);

        if (detalle == null) return null;

        var receta = await _contexto.Receta
            .Include(r => r.RecetaIngredientes)
            .ThenInclude(ri => ri.Ingrediente)
            .FirstOrDefaultAsync(r => r.IdPlatillos == detalle.IdPlatillos);

        if (receta == null) return null;

        return new DetalleRecetaDto
        {
            NombrePlatillo = detalle.Platillo.Nombre,
            Ingredientes = receta.RecetaIngredientes.Select(ri => new IngredienteRecetaDto
            {
                Nombre = ri.Ingrediente.Nombre,
                UnidadMedida = ri.Ingrediente.UnidadDeMedida.ToString(),
                Cantidad = ri.Cantidad * detalle.Cantidad
            }).ToList()
        };
    }

    public async Task<(bool Exitoso, string Mensaje)> IniciarPreparacionAsync(int idDetalle)
    {
        var detalle = await _contexto.PedidoDetalle
            .Include(d => d.Platillo)
            .FirstOrDefaultAsync(d => d.Id == idDetalle);

        if (detalle == null) return (false, "Platillo no encontrado.");

        if (detalle.Estado != EstadoDePlatilloEnCocina.Pendiente)
            return (false, "Solo se puede iniciar un platillo en estado Pendiente de preparar.");

        var receta = await _contexto.Receta
            .FirstOrDefaultAsync(r => r.IdPlatillos == detalle.IdPlatillos);

        if (receta == null) return (false, "No existe una receta para este platillo.");

        var ingredientesReceta = await _contexto.RecetaDetalleIngredientes
            .Include(dri => dri.Ingrediente)
            .Where(dri => dri.IdReceta == receta.Id)
            .ToListAsync();

        foreach (var item in ingredientesReceta)
        {
            double cantidadNecesaria = item.Cantidad * detalle.Cantidad;
            if (item.Ingrediente.CantiadadDisponible < cantidadNecesaria)
            {
                return (false, $"Inventario insuficiente de {item.Ingrediente.Nombre}. Disponible: {item.Ingrediente.CantiadadDisponible}");
            }
        }

        foreach (var item in ingredientesReceta)
        {
            item.Ingrediente.CantiadadDisponible -= (item.Cantidad * detalle.Cantidad);
        }

        detalle.Estado = EstadoDePlatilloEnCocina.EnPreparacion;
        detalle.FechaDeInicioPreparacion = DateTime.Now;

        var pedido = await _contexto.Pedido.FindAsync(detalle.IdPedido);
        if (pedido != null && pedido.Estado == EstadoPedido.Pendiente)
        {
            pedido.Estado = EstadoPedido.EnPreparacion;
        }

        await _contexto.SaveChangesAsync();
        return (true, "Preparación iniciada correctamente.");
    }

    public async Task<bool> MarcarPedidoComoAtendidoAsync(int idDetalle, string? observaciones = null)
    {
        var detalle = await _contexto.PedidoDetalle.FindAsync(idDetalle);
        if (detalle == null || detalle.Estado != EstadoDePlatilloEnCocina.EnPreparacion)
            return false;

        detalle.Estado = EstadoDePlatilloEnCocina.Atendido;
        detalle.FechaFinalDePreparacion = DateTime.Now;
        if (!string.IsNullOrWhiteSpace(observaciones))
        {
            detalle.ObservacionesCocina = observaciones;
        }

        await _contexto.SaveChangesAsync();
        return true;
    }
}