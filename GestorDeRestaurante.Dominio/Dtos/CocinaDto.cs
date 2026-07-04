using GestorDeRestaurante.Dominio.Entidades;

namespace GestorDeRestaurante.Dominio.Dtos;

public class CocinaDto
{
    public class ListarPedidosPendientesDto
    {
    public int Id { get; set; }
    public int IdPedido { get; set; }
    public DateTime FechaDeRegistro { get; set; }
    public string NombreDelPlatillo { get; set; } = null!;
    public int Cantidad { get; set; }
    public string ObservacionDelPlatillo { get; set; } = null!;
    public TipoDePedido TipoDePedido { get; set; }
    public string NombreDelCliente { get; set; } = null!;
    public string ObservacionesPedido { get; set; } = null!;
    public int NumeroDeMesa { get; set; }
    }

    public class ListarPedidosEnPreparacionDto
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public string NombreDelPlatillo { get; set; } = null!;
        public int Cantidad { get; set; }
        public DateTime FechaDeInicio { get; set; }
        public TimeSpan TiempoTranscurrido { get; set; }
        public string? Observaciones { get; set; }
        public string NombreDelCliente { get; set; } = null!;
        public string ObservacionesPedido { get; set; } = null!;
        public TipoDePedido TipoDePedido { get; set; }
        public int NumeroDeMesa { get; set; }
    }

    public class ListarPedidosAtendidosDto
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public string NombreDelPlatillo { get; set; } = null!;
        public int Cantidad { get; set; }
        public DateTime FechaDeAtencion { get; set; }
        public string NombreDelCliente { get; set; } = null!;
        public TipoDePedido TipoDePedido { get; set; }
    }

    public class DetalleRecetaDto
    {
        public string NombrePlatillo { get; set; } = null!;
        public List<IngredienteRecetaDto> Ingredientes { get; set; } = new();
    }

    public class IngredienteRecetaDto
    {
        public string Nombre { get; set; } = null!;
        public string UnidadMedida { get; set; } = null!;
        public double Cantidad { get; set; }
    }
}
