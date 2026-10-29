using Cepheus.API.Endpoints.Comunes;
using Cepheus.API.Endpoints.Logistica.Catalogos;
using Cepheus.API.Endpoints.Logistica.Maestros;

namespace Cepheus.API.Extensions.Endpoints.Logistica;

public static class LogisticaCatalogosEndpointsExtensions
{
    public static WebApplication MapLogisticaCatalogosEndpoints(
        this WebApplication app)
    {
        app.MapFamiliasEndpoints();
        app.MapSubFamiliasEndpoints();
        app.MapUnidadesMedidaEndpoints();
        app.MapTiposCompraEndpoints();
        app.MapNotasCompraEndpoints();
        app.MapLugaresEnvioEndpoints();
        app.MapCompradoresEndpoints();
        app.MapTramitesEndpoints();
        app.MapTiposPedidoEndpoints();
        app.MapUnidadesNegocioEndpoints();
        app.MapTiposValeEndpoints();
        app.MapTiposArticuloEndpoints();
        app.MapPlanesArticuloEndpoints();
        app.MapFormasPagoEndpoints();
        app.MapNivelesEndpoints();
        app.MapTiposTransaccionEndpoints();
        app.MapRangosAprobacionEndpoints();
        app.MapAprobadoresAsignadosEndpoints();
        app.MapMotivosDevolucionArticuloEndpoints();
        app.MapUsuariosPlantaEndpoints();

        return app;
    }
}