using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDeRestaurante.Dominio.Dtos
{
    public class DesglosePedidoDto
    {
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
    }
}
