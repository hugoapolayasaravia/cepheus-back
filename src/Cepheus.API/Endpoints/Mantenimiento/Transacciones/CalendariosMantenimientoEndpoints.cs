using Cepheus.Application.Features.Mantenimiento.Transacciones.CalendarioEquipos.GetReporteCalendarioEquipos;
using Cepheus.Application.Features.Mantenimiento.Transacciones.CalendarioOrdenesEquipo.GetReporteCalendarioOrdenesEquipo;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones;

public static class CalendariosMantenimientoEndpoints
{
    public static void MapCalendariosMantenimientoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/mantenimiento/transacciones")
            .WithTags("Calendarios (Mantenimiento)")
            .RequireAuthorization();

        // Calendario de equipos: dato diario de cada plan preventivo. plantaCode/equipoCode: omitir = todos.
        group.MapGet("/calendario-equipos", async (
            string? plantaCode, string? equipoCode, DateTime fechaDesde, DateTime fechaHasta, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteCalendarioEquiposQuery(
                    plantaCode, equipoCode, fechaDesde, fechaHasta))))
        .WithName("GetReporteCalendarioEquipos")
        .RequireAuthorization("CALENDARIOEQUIPOS.VIEW");

        // Calendario de órdenes de trabajo por equipo en un rango de fechas. plantaCode/equipoCode/estado: omitir = todos.
        group.MapGet("/calendario-ordenes-equipo", async (
            string? plantaCode, string? equipoCode, EstadoOrdenTrabajo? estado,
            DateTime fechaDesde, DateTime fechaHasta, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteCalendarioOrdenesEquipoQuery(
                    plantaCode, equipoCode, estado, fechaDesde, fechaHasta))))
        .WithName("GetReporteCalendarioOrdenesEquipo")
        .RequireAuthorization("CALENDARIOOT.VIEW");
    }
}
