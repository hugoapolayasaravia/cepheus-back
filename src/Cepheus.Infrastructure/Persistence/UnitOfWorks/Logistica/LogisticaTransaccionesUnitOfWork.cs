// Cepheus.Infrastructure/Persistence/UnitOfWorks/LogisticaTransaccionesUnitOfWork.cs
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Logistica
{
    public sealed class LogisticaTransaccionesUnitOfWork : ILogisticaTransaccionesUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public LogisticaTransaccionesUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Pedido>? _pedidos;
        public IRepository<Pedido> Pedidos => _pedidos ??= new Repository<Pedido>(_context);

        private IRepository<PedidoDetalle>? _pedidoDetalles;
        public IRepository<PedidoDetalle> PedidoDetalles => _pedidoDetalles ??= new Repository<PedidoDetalle>(_context);

        private IRepository<Cotizacion>? _cotizaciones;
        public IRepository<Cotizacion> Cotizaciones => _cotizaciones ??= new Repository<Cotizacion>(_context);

        private IRepository<CotizacionDetalle>? _cotizacionDetalles;
        public IRepository<CotizacionDetalle> CotizacionDetalles => _cotizacionDetalles ??= new Repository<CotizacionDetalle>(_context);

        private IRepository<CotizacionPedidoOrigen>? _cotizacionPedidoOrigenes;
        public IRepository<CotizacionPedidoOrigen> CotizacionPedidoOrigenes => _cotizacionPedidoOrigenes ??= new Repository<CotizacionPedidoOrigen>(_context);

        private IRepository<CotizacionProveedor>? _cotizacionProveedores;
        public IRepository<CotizacionProveedor> CotizacionProveedores => _cotizacionProveedores ??= new Repository<CotizacionProveedor>(_context);

        private IRepository<CotizacionProveedorDetalle>? _cotizacionProveedorDetalles;
        public IRepository<CotizacionProveedorDetalle> CotizacionProveedorDetalles => _cotizacionProveedorDetalles ??= new Repository<CotizacionProveedorDetalle>(_context);

        private IRepository<OrdenCompra>? _ordenesCompra;
        public IRepository<OrdenCompra> OrdenesCompra => _ordenesCompra ??= new Repository<OrdenCompra>(_context);

        private IRepository<OrdenCompraDetalle>? _ordenCompraDetalles;
        public IRepository<OrdenCompraDetalle> OrdenCompraDetalles => _ordenCompraDetalles ??= new Repository<OrdenCompraDetalle>(_context);

        private IRepository<OrdenCompraPedidoOrigen>? _ordenCompraPedidoOrigenes;
        public IRepository<OrdenCompraPedidoOrigen> OrdenCompraPedidoOrigenes => _ordenCompraPedidoOrigenes ??= new Repository<OrdenCompraPedidoOrigen>(_context);

        private IRepository<Guia>? _guias;
        public IRepository<Guia> Guias => _guias ??= new Repository<Guia>(_context);

        private IRepository<GuiaDetalle>? _guiaDetalles;
        public IRepository<GuiaDetalle> GuiaDetalles => _guiaDetalles ??= new Repository<GuiaDetalle>(_context);

        private IRepository<NotaIngreso>? _notaIngresos;
        public IRepository<NotaIngreso> NotaIngresos => _notaIngresos ??= new Repository<NotaIngreso>(_context);

        private IRepository<NotaIngresoDetalle>? _notaIngresoDetalles;
        public IRepository<NotaIngresoDetalle> NotaIngresoDetalles => _notaIngresoDetalles ??= new Repository<NotaIngresoDetalle>(_context);

        private IRepository<Importacion>? _importaciones;
        public IRepository<Importacion> Importaciones => _importaciones ??= new Repository<Importacion>(_context);

        private IRepository<ImportacionDetalle>? _importacionDetalles;
        public IRepository<ImportacionDetalle> ImportacionDetalles => _importacionDetalles ??= new Repository<ImportacionDetalle>(_context);

        private IRepository<ImportacionGasto>? _importacionGastos;
        public IRepository<ImportacionGasto> ImportacionGastos => _importacionGastos ??= new Repository<ImportacionGasto>(_context);

        private IRepository<OrdenServicio>? _ordenesServicio;
        public IRepository<OrdenServicio> OrdenesServicio => _ordenesServicio ??= new Repository<OrdenServicio>(_context);

        private IRepository<OrdenServicioDetalle>? _ordenServicioDetalles;
        public IRepository<OrdenServicioDetalle> OrdenServicioDetalles => _ordenServicioDetalles ??= new Repository<OrdenServicioDetalle>(_context);

        private IRepository<OrdenServicioSalida>? _ordenesServicioSalida;
        public IRepository<OrdenServicioSalida> OrdenesServicioSalida => _ordenesServicioSalida ??= new Repository<OrdenServicioSalida>(_context);

        private IRepository<OrdenServicioSalidaDetalle>? _ordenServicioSalidaDetalles;
        public IRepository<OrdenServicioSalidaDetalle> OrdenServicioSalidaDetalles => _ordenServicioSalidaDetalles ??= new Repository<OrdenServicioSalidaDetalle>(_context);

        private IRepository<Vale>? _vales;
        public IRepository<Vale> Vales => _vales ??= new Repository<Vale>(_context);

        private IRepository<ValeDetalle>? _valeDetalles;
        public IRepository<ValeDetalle> ValeDetalles => _valeDetalles ??= new Repository<ValeDetalle>(_context);

        private IRepository<AjusteInventario>? _ajustesInventario;
        public IRepository<AjusteInventario> AjustesInventario => _ajustesInventario ??= new Repository<AjusteInventario>(_context);

        private IRepository<AjusteInventarioDetalle>? _ajusteInventarioDetalles;
        public IRepository<AjusteInventarioDetalle> AjusteInventarioDetalles => _ajusteInventarioDetalles ??= new Repository<AjusteInventarioDetalle>(_context);

        private IRepository<MovimientoArticulo>? _movimientosArticulo;
        public IRepository<MovimientoArticulo> MovimientosArticulo => _movimientosArticulo ??= new Repository<MovimientoArticulo>(_context);

        private IRepository<SaldoMensualArticulo>? _saldosMensualesArticulo;
        public IRepository<SaldoMensualArticulo> SaldosMensualesArticulo => _saldosMensualesArticulo ??= new Repository<SaldoMensualArticulo>(_context);

        private IRepository<CierreIncidencia>? _cierreIncidencias;
        public IRepository<CierreIncidencia> CierreIncidencias => _cierreIncidencias ??= new Repository<CierreIncidencia>(_context);

        private IRepository<CierreRevaluacion>? _cierreRevaluaciones;
        public IRepository<CierreRevaluacion> CierreRevaluaciones => _cierreRevaluaciones ??= new Repository<CierreRevaluacion>(_context);

        private IRepository<CierreBloqueo>? _cierreBloqueos;
        public IRepository<CierreBloqueo> CierreBloqueos => _cierreBloqueos ??= new Repository<CierreBloqueo>(_context);

        private IRepository<OrdenCompraAdministracion>? _ordenesCompraAdministracion;
        public IRepository<OrdenCompraAdministracion> OrdenesCompraAdministracion => _ordenesCompraAdministracion ??= new Repository<OrdenCompraAdministracion>(_context);

        private IRepository<OrdenCompraAdministracionDetalle>? _ordenCompraAdministracionDetalles;
        public IRepository<OrdenCompraAdministracionDetalle> OrdenCompraAdministracionDetalles => _ordenCompraAdministracionDetalles ??= new Repository<OrdenCompraAdministracionDetalle>(_context);
    }
}