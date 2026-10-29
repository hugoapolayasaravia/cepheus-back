using Cepheus.API.Endpoints.Mantenimiento.Catalogos;

namespace Cepheus.API.Extensions.Endpoints.Mantenimiento;

public static class MantenimientoCatalogosEndpointsExtensions
{
    public static WebApplication MapMantenimientoCatalogosEndpoints(
        this WebApplication app)
    {
        app.MapInspeccionesEndpoints();
        app.MapEspecialidadesEndpoints();
        app.MapOportunidadesEndpoints();
        app.MapPrioridadesEndpoints();
        app.MapTiposOrdenEndpoints();
        app.MapMaquinasEndpoints();

        return app;
    }
}