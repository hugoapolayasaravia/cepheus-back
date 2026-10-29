using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Facturacion.Transacciones;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks
{
    public sealed class FacturacionTransaccionesUnitOfWork
        : IFacturacionTransaccionesUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public FacturacionTransaccionesUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRepository<Cotizacion>? _cotizaciones;
        public IRepository<Cotizacion> Cotizaciones => _cotizaciones ??= new Repository<Cotizacion>(_context);

        private IRepository<CotizacionDetalle>? _cotizacionesDetalle;
        public IRepository<CotizacionDetalle> CotizacionesDetalle => _cotizacionesDetalle ??= new Repository<CotizacionDetalle>(_context);

        private IRepository<CotizacionNota>? _cotizacionesNotas;
        public IRepository<CotizacionNota> CotizacionesNotas => _cotizacionesNotas ??= new Repository<CotizacionNota>(_context);

        private IRepository<CotizacionMetradoResumen>? _cotizacionesMetradoResumen;
        public IRepository<CotizacionMetradoResumen> CotizacionesMetradoResumen => _cotizacionesMetradoResumen ??= new Repository<CotizacionMetradoResumen>(_context);

        private IRepository<CotizacionMetradoDetalle>? _cotizacionesMetradoDetalle;
        public IRepository<CotizacionMetradoDetalle> CotizacionesMetradoDetalle => _cotizacionesMetradoDetalle ??= new Repository<CotizacionMetradoDetalle>(_context);

        private IRepository<HojaCorte>? _hojasCorte;
        public IRepository<HojaCorte> HojasCorte => _hojasCorte ??= new Repository<HojaCorte>(_context);

        private IRepository<HojaCorteDetalle>? _hojasCorteDetalle;
        public IRepository<HojaCorteDetalle> HojasCorteDetalle => _hojasCorteDetalle ??= new Repository<HojaCorteDetalle>(_context);

        private IRepository<OrdenTrabajoVenta>? _ordenesTrabajoVenta;
        public IRepository<OrdenTrabajoVenta> OrdenesTrabajoVenta => _ordenesTrabajoVenta ??= new Repository<OrdenTrabajoVenta>(_context);

        private IRepository<OrdenTrabajoVentaDetalle>? _ordenesTrabajoVentaDetalle;
        public IRepository<OrdenTrabajoVentaDetalle> OrdenesTrabajoVentaDetalle => _ordenesTrabajoVentaDetalle ??= new Repository<OrdenTrabajoVentaDetalle>(_context);

        private IRepository<OrdenTrabajoVentaAprobacion>? _ordenesTrabajoVentaAprobaciones;
        public IRepository<OrdenTrabajoVentaAprobacion> OrdenesTrabajoVentaAprobaciones => _ordenesTrabajoVentaAprobaciones ??= new Repository<OrdenTrabajoVentaAprobacion>(_context);

        private IRepository<OrdenTrabajoVentaAprobacionAnulada>? _ordenesTrabajoVentaAprobacionesAnuladas;
        public IRepository<OrdenTrabajoVentaAprobacionAnulada> OrdenesTrabajoVentaAprobacionesAnuladas => _ordenesTrabajoVentaAprobacionesAnuladas ??= new Repository<OrdenTrabajoVentaAprobacionAnulada>(_context);

        private IRepository<ProgramacionConcreto>? _programacionesConcreto;
        public IRepository<ProgramacionConcreto> ProgramacionesConcreto => _programacionesConcreto ??= new Repository<ProgramacionConcreto>(_context);

        private IRepository<ProgramacionConcretoDetalle>? _programacionesConcretoDetalle;
        public IRepository<ProgramacionConcretoDetalle> ProgramacionesConcretoDetalle => _programacionesConcretoDetalle ??= new Repository<ProgramacionConcretoDetalle>(_context);

        private IRepository<ProgramacionOtros>? _programacionesOtros;
        public IRepository<ProgramacionOtros> ProgramacionesOtros => _programacionesOtros ??= new Repository<ProgramacionOtros>(_context);

        private IRepository<ProgramacionOtrosDetalle>? _programacionesOtrosDetalle;
        public IRepository<ProgramacionOtrosDetalle> ProgramacionesOtrosDetalle => _programacionesOtrosDetalle ??= new Repository<ProgramacionOtrosDetalle>(_context);

        private IRepository<GuiaRemisionVenta>? _guiasRemisionVenta;
        public IRepository<GuiaRemisionVenta> GuiasRemisionVenta => _guiasRemisionVenta ??= new Repository<GuiaRemisionVenta>(_context);

        private IRepository<GuiaRemisionVentaDetalle>? _guiasRemisionVentaDetalle;
        public IRepository<GuiaRemisionVentaDetalle> GuiasRemisionVentaDetalle => _guiasRemisionVentaDetalle ??= new Repository<GuiaRemisionVentaDetalle>(_context);

        private IRepository<ComprobanteVenta>? _comprobantesVenta;
        public IRepository<ComprobanteVenta> ComprobantesVenta => _comprobantesVenta ??= new Repository<ComprobanteVenta>(_context);

        private IRepository<ComprobanteVentaDetalle>? _comprobantesVentaDetalle;
        public IRepository<ComprobanteVentaDetalle> ComprobantesVentaDetalle => _comprobantesVentaDetalle ??= new Repository<ComprobanteVentaDetalle>(_context);

        private IRepository<SolicitudNotaCredito>? _solicitudesNotaCredito;
        public IRepository<SolicitudNotaCredito> SolicitudesNotaCredito => _solicitudesNotaCredito ??= new Repository<SolicitudNotaCredito>(_context);

        private IRepository<SolicitudNotaCreditoDetalle>? _solicitudesNotaCreditoDetalle;
        public IRepository<SolicitudNotaCreditoDetalle> SolicitudesNotaCreditoDetalle => _solicitudesNotaCreditoDetalle ??= new Repository<SolicitudNotaCreditoDetalle>(_context);

        private IRepository<SolicitudNotaCreditoGuia>? _solicitudesNotaCreditoGuias;
        public IRepository<SolicitudNotaCreditoGuia> SolicitudesNotaCreditoGuias => _solicitudesNotaCreditoGuias ??= new Repository<SolicitudNotaCreditoGuia>(_context);

        private IRepository<SolicitudNotaCreditoArchivo>? _solicitudesNotaCreditoArchivos;
        public IRepository<SolicitudNotaCreditoArchivo> SolicitudesNotaCreditoArchivos => _solicitudesNotaCreditoArchivos ??= new Repository<SolicitudNotaCreditoArchivo>(_context);

        private IRepository<SolicitudNotaCreditoAprobacion>? _solicitudesNotaCreditoAprobaciones;
        public IRepository<SolicitudNotaCreditoAprobacion> SolicitudesNotaCreditoAprobaciones => _solicitudesNotaCreditoAprobaciones ??= new Repository<SolicitudNotaCreditoAprobacion>(_context);

        private IRepository<NotaCreditoVenta>? _notasCreditoVenta;
        public IRepository<NotaCreditoVenta> NotasCreditoVenta => _notasCreditoVenta ??= new Repository<NotaCreditoVenta>(_context);

        private IRepository<NotaCreditoVentaDetalle>? _notasCreditoVentaDetalle;
        public IRepository<NotaCreditoVentaDetalle> NotasCreditoVentaDetalle => _notasCreditoVentaDetalle ??= new Repository<NotaCreditoVentaDetalle>(_context);

    
    }
}