using SistemaAlquilerPlaya.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaAlquilerPlaya.Dominio.Entidades
{
    public class Alquiler
    {
        public int Id { get; set; }
        public int ArticuloId { get; set; }
        public string IdentificacionCliente { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public DateTime FechaAlquiler { get; set; }
        public DateTime HoraInicio { get; set; }
        public int CantidadHoras { get; set; }
        public decimal MontoTotal { get; set; }
        public Articulo? Articulo { get; set; }
    }
}