using Cepheus.API.Endpoints.Mantenimiento.Catalogos;
using Cepheus.API.Endpoints.Mantenimiento.Maestros;

namespace Cepheus.API.Extensions.Endpoints.Mantenimiento;

public static class MantenimientoMaestrosEndpointsExtensions
{
    public static WebApplication MapMantenimientoMaestrosEndpoints(
        this WebApplication app)
    {
        app.MapEquiposEndpoints();
        app.MapCentrosEjecutoresEndpoints();
        app.MapSubCentrosEjecutoresEndpoints();
        app.MapVerbosActividadEndpoints();
        app.MapObjetosActividadEndpoints();
        app.MapActividadesEndpoints();

        return app;
    }
}