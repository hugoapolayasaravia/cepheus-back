using Cepheus.API.Endpoints.Mantenimiento.Transacciones;

namespace Cepheus.API.Extensions.Endpoints.Mantenimiento;

public static class MantenimientoTransaccionesEndpointsExtensions
{
    public static WebApplication MapMantenimientoTransaccionesEndpoints(
        this WebApplication app)
    {
        app.MapOrdenesTrabajoEndpoints();
        app.MapOTResponsablesEndpoints();
        app.MapOTRMaquinasEndpoints();
        app.MapOTRMaterialesEndpoints();

        return app;
    }
}
