using static GestorDeRestaurante.Dominio.Dtos.CocinaDto;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces;

public interface IServicioDeCocina
{
    Task<List<ListarPedidosPendientesDto>> ListarPedidosPendientesAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null);
    Task<List<ListarPedidosEnPreparacionDto>> ListarPedidosEnPreparacionAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null);
    Task<List<ListarPedidosAtendidosDto>> ListarPedidosAtendidosAsync(string? filtroNombre = null, int? filtrarPedido = null, string? filtrarTipo = null);
    Task<DetalleRecetaDto?> ObtenerDetallesRecetaAsync(int idDetalle);
    Task<(bool Exitoso, string Mensaje)> IniciarPreparacionAsync(int detalleId);
    Task<bool> MarcarPedidoComoAtendidoAsync(int idDetalle, string? observaciones = null);
}