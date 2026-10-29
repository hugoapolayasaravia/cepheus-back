using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Logistica - Transacciones
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();

    public DbSet<Cotizacion> CotizacionesProveedor => Set<Cotizacion>();
    public DbSet<CotizacionDetalle> CotizacionesDetalleProveedor => Set<CotizacionDetalle>();
    public DbSet<CotizacionPedidoOrigen> CotizacionPedidosOrigen => Set<CotizacionPedidoOrigen>();
    public DbSet<CotizacionProveedor> CotizacionProveedores => Set<CotizacionProveedor>();
    public DbSet<CotizacionProveedorDetalle> CotizacionProveedoresDetalle => Set<CotizacionProveedorDetalle>();

    public DbSet<OrdenCompra> OrdenCompras => Set<OrdenCompra>();
    public DbSet<OrdenCompraDetalle> OrdenComprasDetalle => Set<OrdenCompraDetalle>();
    public DbSet<OrdenCompraPedidoOrigen> OrdenCompraPedidosOrigen => Set<OrdenCompraPedidoOrigen>();

    public DbSet<Guia> Guias => Set<Guia>();
    public DbSet<GuiaDetalle> GuiaDetalles => Set<GuiaDetalle>();

    public DbSet<NotaIngreso> NotaIngresos => Set<NotaIngreso>();
    public DbSet<NotaIngresoDetalle> NotaIngresoDetalles => Set<NotaIngresoDetalle>();

    public DbSet<Importacion> Importaciones => Set<Importacion>();
    public DbSet<ImportacionDetalle> ImportacionDetalles => Set<ImportacionDetalle>();
    public DbSet<ImportacionGasto> ImportacionGastos => Set<ImportacionGasto>();

    public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
    public DbSet<OrdenServicioDetalle> OrdenServicioDetalles => Set<OrdenServicioDetalle>();

    public DbSet<OrdenServicioSalida> OrdenesServicioSalida => Set<OrdenServicioSalida>();
    public DbSet<OrdenServicioSalidaDetalle> OrdenServicioSalidaDetalles => Set<OrdenServicioSalidaDetalle>();

    public DbSet<Vale> Vales => Set<Vale>();
    public DbSet<ValeDetalle> ValeDetalles => Set<ValeDetalle>();

    public DbSet<AjusteInventario> AjustesInventario => Set<AjusteInventario>();
    public DbSet<AjusteInventarioDetalle> AjusteInventarioDetalles => Set<AjusteInventarioDetalle>();

    public DbSet<MovimientoArticulo> MovimientosArticulo => Set<MovimientoArticulo>();
    public DbSet<SaldoMensualArticulo> SaldosMensualesArticulo => Set<SaldoMensualArticulo>();
    public DbSet<CierreIncidencia> CierreIncidencias => Set<CierreIncidencia>();
    public DbSet<CierreRevaluacion> CierreRevaluaciones => Set<CierreRevaluacion>();
    public DbSet<CierreBloqueo> CierreBloqueos => Set<CierreBloqueo>();

    public DbSet<OrdenCompraAdministracion> OrdenesCompraAdministracion => Set<OrdenCompraAdministracion>();
    public DbSet<OrdenCompraAdministracionDetalle> OrdenCompraAdministracionDetalles => Set<OrdenCompraAdministracionDetalle>();
}