using Cepheus.API.Endpoints.Mantenimiento.Reportes;

namespace Cepheus.API.Extensions.Endpoints.Mantenimiento;

public static class MantenimientoReportesEndpointsExtensions
{
    public static WebApplication MapMantenimientoReportesEndpoints(this WebApplication app)
    {
        app.MapReportesMantenimientoEndpoints();

        return app;
    }
}
