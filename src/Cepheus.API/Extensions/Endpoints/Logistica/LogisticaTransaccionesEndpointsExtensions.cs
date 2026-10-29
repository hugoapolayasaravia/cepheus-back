using Cepheus.API.Endpoints.Logistica.Transacciones;

namespace Cepheus.API.Extensions.Endpoints.Logistica;

public static class LogisticaTransaccionesEndpointsExtensions
{
    public static WebApplication MapLogisticaTransaccionesEndpoints(this WebApplication app)
    {
        app.MapPedidosEndpoints();
        app.MapCotizacionesEndpoints();
        app.MapOrdenesCompraEndpoints();
        app.MapGuiasEndpoints();
        app.MapNotaIngresosEndpoints();
        app.MapImportacionesEndpoints();
        app.MapOrdenesServicioEndpoints();
        app.MapOrdenesServicioSalidaEndpoints();
        app.MapValesEndpoints();
        app.MapAjustesInventarioEndpoints();

        app.MapTransferenciasEndpoints();
        app.MapKardexEndpoints();
        app.MapCierresMensualesEndpoints();
        app.MapReporteCierreEndpoints();
        app.MapOrdenesCompraAdministracionEndpoints();


        return app;
    }
}