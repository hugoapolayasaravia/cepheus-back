

using Cepheus.Domain.Facturacion.Transacciones;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IFacturacionTransaccionesUnitOfWork
    {

        IRepository<Cotizacion> Cotizaciones { get; }
        IRepository<CotizacionDetalle> CotizacionesDetalle { get; }
        IRepository<CotizacionNota> CotizacionesNotas { get; }
        IRepository<CotizacionMetradoResumen> CotizacionesMetradoResumen { get; }
        IRepository<CotizacionMetradoDetalle> CotizacionesMetradoDetalle { get; }

        IRepository<HojaCorte> HojasCorte { get; }
        IRepository<HojaCorteDetalle> HojasCorteDetalle { get; }

        IRepository<OrdenTrabajoVenta> OrdenesTrabajoVenta { get; }
        IRepository<OrdenTrabajoVentaDetalle> OrdenesTrabajoVentaDetalle { get; }
        IRepository<OrdenTrabajoVentaAprobacion> OrdenesTrabajoVentaAprobaciones { get; }
        IRepository<OrdenTrabajoVentaAprobacionAnulada> OrdenesTrabajoVentaAprobacionesAnuladas { get; }

        IRepository<ProgramacionConcreto> ProgramacionesConcreto { get; }
        IRepository<ProgramacionConcretoDetalle> ProgramacionesConcretoDetalle { get; }

        IRepository<ProgramacionOtros> ProgramacionesOtros { get; }
        IRepository<ProgramacionOtrosDetalle> ProgramacionesOtrosDetalle { get; }

        IRepository<GuiaRemisionVenta> GuiasRemisionVenta { get; }
        IRepository<GuiaRemisionVentaDetalle> GuiasRemisionVentaDetalle { get; }

        IRepository<ComprobanteVenta> ComprobantesVenta { get; }
        IRepository<ComprobanteVentaDetalle> ComprobantesVentaDetalle { get; }

        IRepository<SolicitudNotaCredito> SolicitudesNotaCredito { get; }
        IRepository<SolicitudNotaCreditoDetalle> SolicitudesNotaCreditoDetalle { get; }
        IRepository<SolicitudNotaCreditoGuia> SolicitudesNotaCreditoGuias { get; }
        IRepository<SolicitudNotaCreditoArchivo> SolicitudesNotaCreditoArchivos { get; }
        IRepository<SolicitudNotaCreditoAprobacion> SolicitudesNotaCreditoAprobaciones { get; }

        IRepository<NotaCreditoVenta> NotasCreditoVenta { get; }
        IRepository<NotaCreditoVentaDetalle> NotasCreditoVentaDetalle { get; }
    }
}