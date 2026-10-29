using Cepheus.API.Endpoints.Logistica.Maestros;

namespace Cepheus.API.Extensions.Endpoints.Logistica;

public static class LogisticaMaestrosEndpointsExtensions
{
    public static WebApplication MapLogisticaMaestrosEndpoints(
        this WebApplication app)
    {
        app.MapProveedoresEndpoints();
        app.MapProveedorDireccionesEndpoints();
        app.MapProveedorContactosEndpoints();
        app.MapProveedorCuentasEndpoints();
        app.MapProveedorCondicionesEndpoints();

        app.MapArticulosEndpoints();
        app.MapArticuloProveedoresEndpoints();
        app.MapStockArticulosEndpoints();

        app.MapCentrosCostoEndpoints();
        app.MapSubCentrosCostoEndpoints();

        app.MapTransportistasEndpoints();
        app.MapVehiculosEndpoints();
        app.MapConductoresEndpoints();

        app.MapControlCierresEndpoints();
        app.MapArticuloUbicacionesEndpoints();
        app.MapArticuloPlantaUbicacionesEndpoints();

        app.MapSubFamiliaAnexosEndpoints();
        app.MapArticuloAnexosEndpoints();
        app.MapProveedorCuentasContablesEndpoints();


        return app;
    }
}