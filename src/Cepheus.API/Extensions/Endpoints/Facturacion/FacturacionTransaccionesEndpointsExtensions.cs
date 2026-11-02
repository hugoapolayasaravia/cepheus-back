using Cepheus.API.Endpoints.Facturacion.Transacciones;

namespace Cepheus.API.Extensions.Endpoints.Facturacion;

public static class FacturacionTransaccionesEndpointsExtensions
{
    public static WebApplication MapFacturacionTransaccionesEndpoints(
        this WebApplication app)
    {
        app.MapCotizacionesEndpoints();
        app.MapCotizacionDetallesEndpoints();
        app.MapCotizacionNotasEndpoints();
        app.MapCotizacionMetradoResumenesEndpoints();
        app.MapCotizacionMetradoDetallesEndpoints();
        app.MapHojasCorteEndpoints();
        app.MapSolicitudesNotaCreditoEndpoints();
        app.MapOrdenesTrabajoVentaEndpoints();
        app.MapProgramacionesConcretoEndpoints();
        app.MapProgramacionesOtrosEndpoints();
        app.MapGuiasRemisionVentaEndpoints();
        app.MapComprobantesVentaEndpoints();
        app.MapNotasCreditoVentaEndpoints();
        app.MapNotasDebitoVentaEndpoints();
        app.MapChequesVentaEndpoints();
        app.MapLetrasVentaEndpoints();
        app.MapCobranzasEndpoints();
        app.MapCancelacionesVentaEndpoints();


        return app;
    }
}
