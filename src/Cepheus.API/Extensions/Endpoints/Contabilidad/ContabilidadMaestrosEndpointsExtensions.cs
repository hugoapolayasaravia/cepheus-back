using Cepheus.API.Endpoints.Contabilidad.Maestros;

namespace Cepheus.API.Extensions.Endpoints.Contabilidad;

public static class ContabilidadMaestrosEndpointsExtensions
{
    public static WebApplication MapContabilidadMaestrosEndpoints(
        this WebApplication app)
    {
        app.MapCuentasContablesEndpoints();
        app.MapAnexosEndpoints();
        app.MapCuentasBancariasEndpoints();
        app.MapPeriodosContablesEndpoints();
        app.MapPlantillasAsientoEndpoints();

        return app;
    }
}