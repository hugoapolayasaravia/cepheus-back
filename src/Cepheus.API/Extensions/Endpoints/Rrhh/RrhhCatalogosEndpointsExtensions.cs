using Cepheus.API.Endpoints.Rrhh.Catalogos;

namespace Cepheus.API.Extensions.Endpoints.Rrhh;

public static class RrhhCatalogosEndpointsExtensions
{
    public static WebApplication MapRrhhCatalogosEndpoints(
        this WebApplication app)
    {
        app.MapNacionalidadesEndpoints();
        app.MapSexosEndpoints();
        app.MapEstadosCivilesEndpoints();
        app.MapTiposViaEndpoints();
        app.MapTiposZonaEndpoints();
        app.MapParentescosEndpoints();
        app.MapAreasEndpoints();
        app.MapOcupacionesEndpoints();
        app.MapSubOcupacionesEndpoints();
        app.MapNivelesEducativosEndpoints();
        app.MapGradosInstruccionEndpoints();
        app.MapTitulosEndpoints();
        app.MapEspecialidadesEndpoints();
        app.MapTiposTrabajadorEndpoints();
        app.MapCategoriasTrabajadorEndpoints();
        app.MapEstadosTrabajadorEndpoints();
        app.MapOficinasEndpoints();
        app.MapCargosEndpoints();
        app.MapRegimenesLaboralesEndpoints();
        app.MapTiposContratoEndpoints();
        app.MapTiposExtensionContratoEndpoints();
        app.MapTiposCuentaEndpoints();
        app.MapModosPagoEndpoints();
        app.MapTiposAfiliacionEndpoints();
        app.MapAfpsEndpoints();
        app.MapRegimenesPensionariosEndpoints();
        app.MapTiposPensionEndpoints();
        app.MapTiposSctrEndpoints();
        app.MapSctrSaludEndpoints();
        app.MapSctrPensionEndpoints();
        app.MapEpsEndpoints();
        app.MapSituacionesEpsEndpoints();
        app.MapTiposCentroFormacionEndpoints();
        app.MapModalidadesFormativasEndpoints();
        app.MapNivelesTrabajadorEndpoints();
        app.MapHorariosEndpoints();
        app.MapTiposSangreEndpoints();
        app.MapAlergiasEndpoints();
        

        return app;
    }
}