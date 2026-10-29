using Cepheus.API.Endpoints.Contabilidad.Transacciones;

namespace Cepheus.API.Extensions.Endpoints.Contabilidad;

public static class ContabilidadTransaccionesEndpointsExtensions
{
    public static WebApplication MapContabilidadTransaccionesEndpoints(
        this WebApplication app)
    {
        app.MapDocumentosCompraEndpoints();
        app.MapAsientosContablesEndpoints();
        app.MapAsientosNotaIngresoEndpoints();
        app.MapAsientosOrdenServicioEndpoints();
        app.MapAsientosConsumoEndpoints();
        app.MapCajasChicaEndpoints();
        app.MapDetraccionesEndpoints();

        return app;
    }
}
