using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    // Facturacion - Transacciones (documento Cotización)

    public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
    public DbSet<CotizacionDetalle> CotizacionesDetalle => Set<CotizacionDetalle>();
    public DbSet<CotizacionNota> CotizacionesNotas => Set<CotizacionNota>();
    public DbSet<CotizacionMetradoResumen> CotizacionesMetradoResumen => Set<CotizacionMetradoResumen>();
    public DbSet<CotizacionMetradoDetalle> CotizacionesMetradoDetalle => Set<CotizacionMetradoDetalle>();

    public DbSet<HojaCorte> HojasCorte => Set<HojaCorte>();
    public DbSet<HojaCorteDetalle> HojasCorteDetalle => Set<HojaCorteDetalle>();

    public DbSet<OrdenTrabajoVenta> OrdenesTrabajoVenta => Set<OrdenTrabajoVenta>();
    public DbSet<OrdenTrabajoVentaDetalle> OrdenesTrabajoVentaDetalle => Set<OrdenTrabajoVentaDetalle>();
    public DbSet<OrdenTrabajoVentaAprobacion> OrdenesTrabajoVentaAprobaciones => Set<OrdenTrabajoVentaAprobacion>();
    public DbSet<OrdenTrabajoVentaAprobacionAnulada> OrdenesTrabajoVentaAprobacionesAnuladas => Set<OrdenTrabajoVentaAprobacionAnulada>();

    public DbSet<ProgramacionConcreto> ProgramacionesConcreto => Set<ProgramacionConcreto>();
    public DbSet<ProgramacionConcretoDetalle> ProgramacionesConcretoDetalle => Set<ProgramacionConcretoDetalle>();

    public DbSet<ProgramacionOtros> ProgramacionesOtros => Set<ProgramacionOtros>();
    public DbSet<ProgramacionOtrosDetalle> ProgramacionesOtrosDetalle => Set<ProgramacionOtrosDetalle>();

    public DbSet<GuiaRemisionVenta> GuiasRemisionVenta => Set<GuiaRemisionVenta>();
    public DbSet<GuiaRemisionVentaDetalle> GuiasRemisionVentaDetalle => Set<GuiaRemisionVentaDetalle>();

    public DbSet<ComprobanteVenta> ComprobantesVenta => Set<ComprobanteVenta>();
    public DbSet<ComprobanteVentaDetalle> ComprobantesVentaDetalle => Set<ComprobanteVentaDetalle>();

    public DbSet<SolicitudNotaCredito> SolicitudesNotaCredito => Set<SolicitudNotaCredito>();
    public DbSet<SolicitudNotaCreditoDetalle> SolicitudesNotaCreditoDetalle => Set<SolicitudNotaCreditoDetalle>();
    public DbSet<SolicitudNotaCreditoGuia> SolicitudesNotaCreditoGuias => Set<SolicitudNotaCreditoGuia>();
    public DbSet<SolicitudNotaCreditoArchivo> SolicitudesNotaCreditoArchivos => Set<SolicitudNotaCreditoArchivo>();
    public DbSet<SolicitudNotaCreditoAprobacion> SolicitudesNotaCreditoAprobaciones => Set<SolicitudNotaCreditoAprobacion>();

    public DbSet<NotaCreditoVenta> NotasCreditoVenta => Set<NotaCreditoVenta>();
    public DbSet<NotaCreditoVentaDetalle> NotasCreditoVentaDetalle => Set<NotaCreditoVentaDetalle>();

    public DbSet<NotaDebitoVenta> NotasDebitoVenta => Set<NotaDebitoVenta>();
    public DbSet<NotaDebitoVentaDetalle> NotasDebitoVentaDetalle => Set<NotaDebitoVentaDetalle>();

    public DbSet<ChequeVenta> ChequesVenta => Set<ChequeVenta>();

}