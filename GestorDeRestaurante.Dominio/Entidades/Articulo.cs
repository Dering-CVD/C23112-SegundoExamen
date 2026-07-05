using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaAlquilerPlaya.Dominio.Entidades
{
    public class Articulo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioHora { get; set; }
        // 1 = Disponible y 2 = Alquilado
        public int Estado { get; set; }
    }
}