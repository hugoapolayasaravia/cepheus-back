// Cepheus.Application/Comun/Interfaces/UnitOfWork/ILogisticaTransaccionesUnitOfWork.cs
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface ILogisticaTransaccionesUnitOfWork
    {
        IRepository<Pedido> Pedidos { get; }
        IRepository<PedidoDetalle> PedidoDetalles { get; }
        IRepository<Cotizacion> Cotizaciones { get; }
        IRepository<CotizacionDetalle> CotizacionDetalles { get; }
        IRepository<CotizacionPedidoOrigen> CotizacionPedidoOrigenes { get; }
        IRepository<CotizacionProveedor> CotizacionProveedores { get; }
        IRepository<CotizacionProveedorDetalle> CotizacionProveedorDetalles { get; }
        IRepository<OrdenCompra> OrdenesCompra { get; }
        IRepository<OrdenCompraDetalle> OrdenCompraDetalles { get; }
        IRepository<OrdenCompraPedidoOrigen> OrdenCompraPedidoOrigenes { get; }
        IRepository<Guia> Guias { get; }
        IRepository<GuiaDetalle> GuiaDetalles { get; }
        IRepository<NotaIngreso> NotaIngresos { get; }
        IRepository<NotaIngresoDetalle> NotaIngresoDetalles { get; }
        IRepository<Importacion> Importaciones { get; }
        IRepository<ImportacionDetalle> ImportacionDetalles { get; }
        IRepository<ImportacionGasto> ImportacionGastos { get; }

        IRepository<OrdenServicio> OrdenesServicio { get; }
        IRepository<OrdenServicioDetalle> OrdenServicioDetalles { get; }
        IRepository<OrdenServicioSalida> OrdenesServicioSalida { get; }
        IRepository<OrdenServicioSalidaDetalle> OrdenServicioSalidaDetalles { get; }
        IRepository<Vale> Vales { get; }
        IRepository<ValeDetalle> ValeDetalles { get; }
        IRepository<AjusteInventario> AjustesInventario { get; }
        IRepository<AjusteInventarioDetalle> AjusteInventarioDetalles { get; }

        IRepository<MovimientoArticulo> MovimientosArticulo { get; }
        IRepository<SaldoMensualArticulo> SaldosMensualesArticulo { get; }
        IRepository<CierreIncidencia> CierreIncidencias { get; }
        IRepository<CierreRevaluacion> CierreRevaluaciones { get; }
        IRepository<CierreBloqueo> CierreBloqueos { get; }

        IRepository<OrdenCompraAdministracion> OrdenesCompraAdministracion { get; }
        IRepository<OrdenCompraAdministracionDetalle> OrdenCompraAdministracionDetalles { get; }



    }
}