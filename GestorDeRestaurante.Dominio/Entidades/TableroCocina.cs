using System;
using System.Collections.Generic;
using System.Text;
using static GestorDeRestaurante.Dominio.Dtos.CocinaDto;

namespace GestorDeRestaurante.Dominio.Entidades
{
    public class TableroCocina
    {
        public List<ListarPedidosPendientesDto> PlatillosPendientes { get; set; } = new();
        public List<ListarPedidosEnPreparacionDto> PlatillosEnPreparacion { get; set; } = new();
        public List<ListarPedidosAtendidosDto> PlatillosAtendidos { get; set; } = new();
    }
}
