using Cepheus.API.Endpoints.Administracion;

namespace Cepheus.API.Extensions.Endpoints.Administracion;

public static class AdministracionEndpointsExtensions
{
    public static WebApplication MapAdministracionEndpoints(
        this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapUsersEndpoints();
        app.MapRolesEndpoints();
        app.MapModulosEndpoints();
        app.MapSubmodulosEndpoints();
        app.MapProgramasEndpoints();
        app.MapPermissionsEndpoints();

        return app;
    }
}