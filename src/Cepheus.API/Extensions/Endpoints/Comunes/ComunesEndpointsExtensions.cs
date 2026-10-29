using Cepheus.API.Endpoints.Comun;
using Cepheus.API.Endpoints.Comunes;

namespace Cepheus.API.Extensions.Endpoints.Comunes;

public static class ComunesEndpointsExtensions
{
    public static WebApplication MapComunesEndpoints(
        this WebApplication app)
    {
        app.MapPlantasEndpoints();
        app.MapMonedasEndpoints();
        app.MapTiposDocumentoEndpoints();
        app.MapComprobantesPagoEndpoints();
        app.MapUbigeosEndpoints();
        app.MapTiposCambioEndpoints();
        app.MapControlesVentasEndpoints();
        app.MapMotivosDevolucionEndpoints();
        app.MapBancosEndpoints();
        app.MapNegociosEndpoints();
        app.MapPeriodosIgvEndpoints();

        return app;
    }
}