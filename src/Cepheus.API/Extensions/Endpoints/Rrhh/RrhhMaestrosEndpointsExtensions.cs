using Cepheus.API.Endpoints.Rrhh.Maestros;

namespace Cepheus.API.Extensions.Endpoints.Rrhh;

public static class RrhhMaestrosEndpointsExtensions
{
    public static WebApplication MapRrhhMaestrosEndpoints(
        this WebApplication app)
    {
        app.MapTrabajadoresEndpoints();
        app.MapTrabajadorDocumentosEndpoints();
        app.MapTrabajadorDomiciliosEndpoints();
        app.MapTrabajadorContactosEndpoints();
        app.MapTrabajadorLaboralsEndpoints();
        app.MapTrabajadorContratosEndpoints();
        app.MapTrabajadorFormacionsEndpoints();
        app.MapTrabajadorRemuneracionsEndpoints();
        app.MapTrabajadorCuentaBancariasEndpoints();
        app.MapTrabajadorPensionsEndpoints();
        app.MapTrabajadorSegurosEndpoints();
        app.MapTrabajadorJornadasEndpoints();
        app.MapTrabajadorBeneficiosEndpoints();
        app.MapTrabajadorDependientesEndpoints();
        app.MapTrabajadorSaludsEndpoints();
        app.MapTrabajadorSindicatosEndpoints();
        app.MapTrabajadorAntecedentesEndpoints();
        app.MapTrabajadorVacacionsEndpoints();
        app.MapTrabajadorFiscalsEndpoints();
        app.MapTrabajadorContablesEndpoints();

        return app;
    }
}