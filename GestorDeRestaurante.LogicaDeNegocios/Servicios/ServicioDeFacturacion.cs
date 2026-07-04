using GestorDeRestaurante.AccesoADatos.Contexto;
using GestorDeRestaurante.Dominio.Dtos;
using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios;

public class ServicioDeFacturacion : IServicioDeFacturacion
{
    private readonly DbContexto _contexto;
    private const decimal impuestoFijo = 0.13m;

    public ServicioDeFacturacion(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<List<PedidoFacturableDto>> ObtenerPedidosDisponiblesParaFacturarAsync()
    {
        var pedidos = await _contexto.Pedido.Where(p => p.Estado == EstadoPedido.Atendido).ToListAsync();

        var resultado = new List<PedidoFacturableDto>();

        foreach (var p in pedidos)
        {
            decimal totalPedido = await _contexto.PedidoDetalle
                .Include(d => d.Platillo)
                .Where(d => d.IdPedido == p.Id)
                .SumAsync(d => d.Cantidad * d.Platillo.PrecioDeVenta);

            resultado.Add(new PedidoFacturableDto
            {
                IdPedido = p.Id,
                FechaPedido = p.FechaDeCreacion,
                Cliente = p.NombreDelCliente,
                Mesa = p.NumeroDeMesa,
                TipoPedido = p.TipoDePedido.ToString(),
                Total = totalPedido
            });
        }

        return resultado;
    }

    public async Task<FacturaDto?> PrepararFacturaAsync(int idPedido)
    {
        var pedido = await _contexto.Pedido.FirstOrDefaultAsync(p => p.Id == idPedido);

        if (pedido == null || pedido.Estado != EstadoPedido.Atendido)
        {
            return null;
        }

        var pedidoDetalles = await _contexto.PedidoDetalle
            .Include(d => d.Platillo)
            .Where(d => d.IdPedido == idPedido)
            .ToListAsync();

        decimal subtotal = pedidoDetalles.Sum(d => d.Cantidad * d.Platillo.PrecioDeVenta);
        decimal impuestoCalculado = subtotal * impuestoFijo;
        decimal total = subtotal + impuestoCalculado;

        return new FacturaDto
        {
            IdPedido = pedido.Id,
            Mesa = pedido.NumeroDeMesa,
            TipoPedido = pedido.TipoDePedido.ToString(),
            Subtotal = subtotal,
            Impuesto = impuestoCalculado,
            Total = total,
            Detalles = pedidoDetalles.Select(d => new FacturaDetalleDto
            {
                NombrePlatillo = d.Platillo.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.Platillo.PrecioDeVenta,
                Subtotal = d.Cantidad * d.Platillo.PrecioDeVenta
            }).ToList()
        };
    }

    public async Task<FacturaDto> GenerarFacturaAsync(int idPedido)
    {
        var pedido = await _contexto.Pedido.FirstOrDefaultAsync(p => p.Id == idPedido);

        if (pedido == null || pedido.Estado != EstadoPedido.Atendido)
        {
            throw new Exception("El pedido no existe o no está disponible para facturar (debe estar Atendido).");
        }

        var facturaDto = await PrepararFacturaAsync(idPedido);

        if (facturaDto == null)
        {
            throw new Exception("Error al generar los datos de la factura.");
        }

        pedido.Estado = EstadoPedido.Facturado;
        await _contexto.SaveChangesAsync();

        return facturaDto;
    }
}