using Cepheus.API.Endpoints.Contabilidad.Catalogos;

namespace Cepheus.API.Extensions.Endpoints.Contabilidad;

public static class ContabilidadCatalogosEndpointsExtensions
{
    public static WebApplication MapContabilidadCatalogosEndpoints(
        this WebApplication app)
    {
        app.MapTiposAnexoEndpoints();
        app.MapTiposCuentaContableEndpoints();
        app.MapTiposAsientoEndpoints();

        return app;
    }
}