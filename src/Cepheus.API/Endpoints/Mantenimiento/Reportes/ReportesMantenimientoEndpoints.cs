using Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;
using Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioOrdenesEquipo.GetReporteCalendarioOrdenesEquipo;
using Cepheus.Application.Features.Mantenimiento.Reportes.HorometroCalendario.GetReporteHorometroCalendario;
using Cepheus.Domain.Mantenimiento.Enum;
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

        // Calendario de órdenes de trabajo por equipo en un rango de fechas. plantaCode/equipoCode/estado: omitir = todos.
        group.MapGet("/calendario-ordenes-equipo", async (
            string? plantaCode, string? equipoCode, EstadoOrdenTrabajo? estado,
            DateTime fechaDesde, DateTime fechaHasta, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteCalendarioOrdenesEquipoQuery(
                    plantaCode, equipoCode, estado, fechaDesde, fechaHasta))))
        .WithName("GetReporteCalendarioOrdenesEquipo")
        .RequireAuthorization("REPORTECALENDARIOOT.VIEW");

        // Calendario de equipos: dato diario de cada plan preventivo. plantaCode/equipoCode: omitir = todos.
        group.MapGet("/calendario-equipos", async (
            string? plantaCode, string? equipoCode, DateTime fechaDesde, DateTime fechaHasta, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteCalendarioEquiposQuery(
                    plantaCode, equipoCode, fechaDesde, fechaHasta))))
        .WithName("GetReporteCalendarioEquipos")
        .RequireAuthorization("REPORTECALENDARIOEQUIPOS.VIEW");
    }
}
