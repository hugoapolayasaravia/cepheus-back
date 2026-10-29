using Cepheus.API.Endpoints.Facturacion.Catalogos;
using Cepheus.API.Endpoints.Facturacion.Maestros;

namespace Cepheus.API.Extensions.Endpoints.Facturacion;

public static class FacturacionCatalogosEndpointsExtensions
{
    public static WebApplication MapFacturacionCatalogosEndpoints(
        this WebApplication app)
    {
        app.MapTiposClienteEndpoints();
        app.MapClasificacionesClienteEndpoints();
        app.MapSegmentosVentasEndpoints();
        app.MapAnalisisVentasEndpoints();
        app.MapTiposValorizacionEndpoints();
        app.MapFormasPagoVentaEndpoints();
        app.MapCategoriasProductoEndpoints();
        app.MapTiposBienEndpoints();
        app.MapTiposOperacionEndpoints();
        app.MapAtributosConcretoEndpoints();
        app.MapTiposProductoEndpoints();
        app.MapUnidadesMedidaVentaEndpoints();
        app.MapListasPrecioEndpoints();
        app.MapStockProductosEndpoints();
        app.MapFletesEndpoints();
        app.MapAlturasLosaEndpoints();
        app.MapPreciosProductoEndpoints();
        app.MapNotasCotizacionPlantillaEndpoints();
        app.MapResponsablesObraEndpoints();
        app.MapTiposHojaCorteEndpoints();
        app.MapMotivosSolicitudNotaCreditoEndpoints();
        app.MapElementosConcretoEndpoints();
        return app;
    }
}
