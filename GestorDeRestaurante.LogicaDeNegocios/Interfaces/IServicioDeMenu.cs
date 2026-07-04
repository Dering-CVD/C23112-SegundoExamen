using GestorDeRestaurante.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDeRestaurante.LogicaDeNegocios.Interfaces
{
    public interface IServicioDeMenu
    {
        List<Platillo> ObtengaLaListaDePlatillos();
        List<Platillo> ObtengaLaListaDePlatillosPorNombre(string nombre);
        Platillo ObtengaElPlatilloPorId(int id);
        void AgregarPlatillo(Platillo nuevoPlatillo);
        void EditarPlatillo(Platillo platillo);
    }
}
