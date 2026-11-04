using Cepheus.Application.Features.Mantenimiento.Reportes.HorometroCalendario.GetReporteHorometroCalendario;
using Cepheus.Application.Features.Mantenimiento.Reportes.UnidadesDia.GetReporteUnidadesDia;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Reportes;

public static class ReportesMantenimientoEndpoints
{
    public static void MapReportesMantenimientoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/mantenimiento/reportes")
            .WithTags("Reportes (Mantenimiento)")
            .RequireAuthorization();

        // Unidades del día para captura de horómetro y paradas. centroCostoCode/plantaCode: omitir = todos.
        group.MapGet("/unidades-dia", async (
            string? centroCostoCode, string? plantaCode, DateTime fecha, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteUnidadesDiaQuery(centroCostoCode, plantaCode, fecha))))
        .WithName("GetReporteUnidadesDia")
        .RequireAuthorization("REPORTEUNIDADESDIA.VIEW");

        // Calendario de lecturas de horómetro por unidad en un rango de fechas.
        group.MapGet("/horometro-calendario", async (
            string? centroCostoCode, string? plantaCode, DateTime fechaDesde, DateTime fechaHasta, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteHorometroCalendarioQuery(
                    centroCostoCode, plantaCode, fechaDesde, fechaHasta))))
        .WithName("GetReporteHorometroCalendario")
        .RequireAuthorization("REPORTEHOROMETROCALENDARIO.VIEW");
    }
}
