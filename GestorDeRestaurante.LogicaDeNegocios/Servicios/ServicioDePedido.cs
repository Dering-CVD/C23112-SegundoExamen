using GestorDeRestaurante.Dominio.Entidades;
using GestorDeRestaurante.LogicaDeNegocios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDeRestaurante.LogicaDeNegocios.Servicios
{
    public class ServicioDePedido : IServicioDePedido
    {
        private GestorDeRestaurante.AccesoADatos.Contexto.DbContexto ContextoDeDatos;

        public ServicioDePedido(GestorDeRestaurante.AccesoADatos.Contexto.DbContexto dbContext)
        {
            ContextoDeDatos = dbContext;
        }

        public List<Pedido> ObtenerLaListaDePedidos()
        {
            var reultado = from c in ContextoDeDatos.Pedido
                           select c;
            return reultado.ToList();
        }

        public List<Pedido> ObtengaLaListaDePedidosPorNombreDelCliente(string nombreDelCliente)
        {
            var resultado = from c in ContextoDeDatos.Pedido
                           where c.NombreDelCliente.ToLower().Contains(nombreDelCliente.ToLower())
                            select c;
            return resultado.ToList();
        }
        public List<PedidoDetalle> ObtenerDetalleDePedido(int idPedido)
        {
            return ContextoDeDatos.PedidoDetalle
        .Include(pd => pd.Platillo)
        .Where(pd => pd.IdPedido == idPedido)
        .ToList();
        }
        public Pedido ObtenerPedidoPorId(int id)
        {
            Pedido pedido;
            pedido = ContextoDeDatos.Pedido.Find(id)!;
            return pedido;
        }
        public void CrearPedido(Pedido nuevoPedido)
        {
            try
            {
                nuevoPedido.Estado = EstadoPedido.Pendiente;
                nuevoPedido.FechaDeCreacion = DateTime.Now;
                if (string.IsNullOrEmpty(nuevoPedido.Observaciones))
                {
                    nuevoPedido.Observaciones = string.Empty;
                }

                if (string.IsNullOrEmpty(nuevoPedido.MotivoDeLaCancelacion))
                {
                    nuevoPedido.MotivoDeLaCancelacion = string.Empty;
                }
                ContextoDeDatos.Pedido.Add(nuevoPedido);
                ContextoDeDatos.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error al crear el pedido.", ex);
            }

        }
        public void AgregarPlatiloAlPedido(Platillo platillo, Pedido pedido, int cantidad, string? observaciones = null)
        {
            if (platillo == null)
            {
                throw new ArgumentNullException(nameof(platillo), "El platillo no puede ser nulo.");
            }
            if (pedido == null)
            {
                throw new ArgumentNullException(nameof(pedido), "El pedido no puede ser nulo.");
            }
            var pedidoExistente = ContextoDeDatos.Pedido.Find(pedido.Id);
            if (pedidoExistente == null)
            {
                throw new InvalidOperationException("El pedido no existe en la base de datos.");
            }
            if (cantidad <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser mayor que cero.");
            }
            if (platillo.Estado != EstadoDePlatilloEnMenu.Disponible)
            {
                throw new InvalidOperationException($"El platillo '{platillo.Nombre}' no se encuentra disponible");
            }
            if (pedidoExistente.Estado == EstadoPedido.Cancelado || pedidoExistente.Estado == EstadoPedido.Facturado)
            {
                throw new InvalidOperationException($"No se pueden agregar platillos a un pedido que está en estado '{pedidoExistente.Estado}'. Solo se pueden agregar platillos a pedidos Pendientes, En Preparación o Atendidos.");
            }
            try
            {
                var nuevoPedidoDetalle = new PedidoDetalle
                {
                    IdPedido = pedido.Id,
                    IdPlatillos = platillo.Id,
                    Cantidad = cantidad,
                    Observaciones = observaciones, 
                    Estado = EstadoDePlatilloEnCocina.Pendiente,
                    FechaDeInicioPreparacion = DateTime.Now,
                    ObservacionesCocina = null, 
                    FechaFinalDePreparacion = null
                };
                ContextoDeDatos.PedidoDetalle.Add(nuevoPedidoDetalle);
                pedido.Estado = EstadoPedido.Pendiente;
                ContextoDeDatos.SaveChanges();
            }
            catch (Exception ex)
            {
                string detalleError = ex.InnerException?.Message ?? ex.Message;
                if (ex.InnerException?.InnerException != null)
                {
                    detalleError += " | " + ex.InnerException.InnerException.Message;
                }
                throw new InvalidOperationException($"Ocurrió un error al agregar el platillo al pedido. Detalles: {detalleError}", ex);
            }
        }

        public void EliminarPlatilloDelPedido(int idPedidoDetalle)
        {
            try
            {
                if (idPedidoDetalle <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(idPedidoDetalle), "El ID del detalle del pedido debe ser mayor que cero.");
                }
                var pedidoDetalle = ContextoDeDatos.PedidoDetalle.Find(idPedidoDetalle);
                if (pedidoDetalle == null)
                {
                    throw new InvalidOperationException("El detalle del pedido no existe en la base de datos.");
                }
                var pedido = ContextoDeDatos.Pedido.Find(pedidoDetalle.IdPedido);
                if (pedido == null)
                {
                    throw new InvalidOperationException("El pedido asociado al detalle del pedido no existe en la base de datos.");
                }
                if (pedidoDetalle.Estado == EstadoDePlatilloEnCocina.EnPreparacion ||
                    pedidoDetalle.Estado == EstadoDePlatilloEnCocina.Atendido)
                {
                    throw new InvalidOperationException($"No se puede eliminar un platillo que está en estado '{pedidoDetalle.Estado}'. Solo se pueden eliminar platillos pendientes.");
                }

                ContextoDeDatos.PedidoDetalle.Remove(pedidoDetalle);
                ContextoDeDatos.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error al eliminar el platillo del pedido.", ex);
            }
        }
        public void EditarPedido(Pedido pedidoEditado)
        {
            try
            {
                var pedidoExistente = ContextoDeDatos.Pedido.Find(pedidoEditado.Id);
                if (pedidoExistente == null)
                {
                    throw new InvalidOperationException("El pedido no existe en la base de datos.");
                }
                if (pedidoEditado.Estado == EstadoPedido.Facturado)
                {
                    throw new InvalidOperationException("No se pueden editar los pedidos que ya han sido facturados.");
                }
                pedidoExistente.NombreDelCliente = pedidoEditado.NombreDelCliente;
                pedidoExistente.NumeroDeMesa = pedidoEditado.NumeroDeMesa;
                pedidoExistente.Observaciones = string.IsNullOrEmpty(pedidoEditado.Observaciones) ? string.Empty : pedidoEditado.Observaciones;
                pedidoExistente.TipoDePedido = pedidoEditado.TipoDePedido;
                ContextoDeDatos.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error al editar el pedido.", ex);
            }

        }
        public void CancelarPedido(int idPedido, string motivoDeLaCancelacion, bool ignorarAdvertenciaPlatillosAtendidos = false)
        {
            try
            {
                if (idPedido <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(idPedido), "El ID del pedido debe ser mayor que cero.");
                }
                var pedidoExistente = ContextoDeDatos.Pedido.Find(idPedido);
                if (pedidoExistente == null)
                {
                    throw new InvalidOperationException("El pedido no existe en la base de datos.");
                }
                if (pedidoExistente.Estado == EstadoPedido.Facturado)
                {
                    throw new InvalidOperationException("No se pueden cancelar los pedidos que ya han sido facturados.");
                }
                if (string.IsNullOrWhiteSpace(motivoDeLaCancelacion) ||
                    motivoDeLaCancelacion.Trim().Length < 10 ||
                    motivoDeLaCancelacion.Trim().Length > 500)
                {
                    throw new ArgumentException("El motivo de la cancelación debe tener entre 10 y 500 caracteres.", nameof(motivoDeLaCancelacion));
                }

                var platillosAtendidos = ContextoDeDatos.PedidoDetalle
                    .Any(pd => pd.IdPedido == idPedido && pd.Estado == EstadoDePlatilloEnCocina.Atendido);

                if (platillosAtendidos && !ignorarAdvertenciaPlatillosAtendidos)
                {
                    throw new InvalidOperationException("ADVERTENCIA: El pedido contiene platillos que ya fueron atendidos. ¿Desea continuar?");
                }
                pedidoExistente.Estado = EstadoPedido.Cancelado;
                pedidoExistente.MotivoDeLaCancelacion = motivoDeLaCancelacion.Trim();

                ContextoDeDatos.SaveChanges();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException || ex is ArgumentException))
            {
                throw new InvalidOperationException("Ocurrió un error inesperado al cancelar el pedido.", ex);
            }

        }
        public List<PedidoDetalle> ObtenerDetalleDePedidoConPlatillo(int idPedido)
        {
            var resultado = ContextoDeDatos.PedidoDetalle
                            .Include(pd => pd.Platillo) 
                            .Where(pd => pd.IdPedido == idPedido);

            return resultado.ToList();
        }
        public decimal CalcularTotalDelPedido(int idPedido)
        {
            var total = ContextoDeDatos.PedidoDetalle
                .Where(pd => pd.IdPedido == idPedido)
                .Sum(pd => (decimal?)pd.Cantidad * (pd.Platillo != null ? pd.Platillo.PrecioDeVenta : 0)) ?? 0;

            return total;
        }

        public List<Pedido> ObtenerPedidosPorRangoDeFechas(DateTime fechaInicial, DateTime fechaFinal)
        {
            try
            {
                if (fechaInicial > fechaFinal)
                {
                    throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final.");
                }

                var fechaFinalAjustada = fechaFinal.AddDays(1).AddTicks(-1);

                var resultado = from p in ContextoDeDatos.Pedido        
                               where p.FechaDeCreacion >= fechaInicial && p.FechaDeCreacion <= fechaFinalAjustada
                               orderby p.FechaDeCreacion descending
                               select p;

                return resultado.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error al obtener el historial de pedidos.", ex);
            }
        }
        public List<PedidoDetalle> ObtenerDetalleDePedidoFiltradoPorNombre(int idPedido, string nombrePlatillo)
        {
            try
            {
                var resultado = ContextoDeDatos.PedidoDetalle
                                .Include(pd => pd.Platillo)
                                .Where(pd => pd.IdPedido == idPedido)
                                .ToList();
                if (!string.IsNullOrWhiteSpace(nombrePlatillo))
                {
                    resultado = resultado
                        .Where(d => d.Platillo != null && 
                                   d.Platillo.Nombre.Contains(nombrePlatillo, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error al obtener los detalles del pedido filtrados.", ex);
            }
        }
        public List<Platillo> ObtenerPlatillosDisponiblesFiltradoPorNombre(string nombrePlatillo)
        {
            try
            {
                var resultado = ContextoDeDatos.Platillos
                    .Where(p => p.Estado == EstadoDePlatilloEnMenu.Disponible)
                    .ToList();
                if (!string.IsNullOrWhiteSpace(nombrePlatillo))
                {
                    resultado = resultado
                        .Where(p => p.Nombre.Contains(nombrePlatillo, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error al obtener los platillos disponibles filtrados.", ex);
            }
        }

        public (decimal subtotal, decimal impuesto, decimal total) CalcularDesgloseDePedido(int idPedido, decimal porcentajeImpuesto = 0.13m)
        {
            try
            {
                var subtotal = ContextoDeDatos.PedidoDetalle
                    .Where(pd => pd.IdPedido == idPedido)
                    .Sum(pd => (decimal?)pd.Cantidad * (pd.Platillo != null ? pd.Platillo.PrecioDeVenta : 0)) ?? 0;

                var impuesto = subtotal * porcentajeImpuesto;

                var total = subtotal + impuesto;

                return (subtotal, impuesto, total);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error al calcular el desglose del pedido.", ex);
            }
        }

        public void ActualizarEstadoDelPedido(int idPedidoDetalle)
        {
            var detalle = ContextoDeDatos.PedidoDetalle.Find(idPedidoDetalle);
            if (detalle == null) return;

            var pedido = ContextoDeDatos.Pedido.Find(detalle.IdPedido);
            if (pedido == null) return;

            if (pedido.Estado == EstadoPedido.Facturado || pedido.Estado == EstadoPedido.Cancelado)
                return;

            var totalDetalles = ContextoDeDatos.PedidoDetalle.Count(d => d.IdPedido == detalle.IdPedido);
            var atendidos = ContextoDeDatos.PedidoDetalle.Count(d => d.IdPedido == detalle.IdPedido && d.Estado == EstadoDePlatilloEnCocina.Atendido);

            if (totalDetalles == atendidos)
                pedido.Estado = EstadoPedido.Atendido;
            else if (atendidos > 0)
                pedido.Estado = EstadoPedido.Pendiente;

            ContextoDeDatos.SaveChanges();
        }
    }
}