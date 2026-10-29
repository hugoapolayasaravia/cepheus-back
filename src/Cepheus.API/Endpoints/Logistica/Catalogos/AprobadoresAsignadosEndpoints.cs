using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.CreateAprobadorAsignado;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.DeleteAprobadorAsignado;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresAsignadosPaginated;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresPorCombinacion;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.UpdateAprobadorAsignado;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class AprobadoresAsignadosEndpoints
    {
        public static void MapAprobadoresAsignadosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/aprobadores-asignados")
                .WithTags("Aprobadores Asignados (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAprobadorAsignadoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/maestros/aprobadores-asignados/{result.NivelCode}/{result.TipoTransaccionCode}/{result.UnidadNegocioCode}/{result.MonedaCode}/{result.TrabajadorCode}",
                    result);
            })
            .WithName("CreateAprobadorAsignado")
            .RequireAuthorization("APROBADORESASIGNADOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAprobadoresAsignadosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAprobadoresAsignadosPagedQueryString")
            .RequireAuthorization("APROBADORESASIGNADOS.VIEW");

            group.MapPost("/paged/body", async (
                GetAprobadoresAsignadosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAprobadoresAsignadosPagedBody")
            .RequireAuthorization("APROBADORESASIGNADOS.VIEW");

            // El uso más natural: "¿quiénes pueden aprobar esta combinación?"
            group.MapGet("/{codigoNivel}/{codigoTrans}/{codigoUne}/{codigoMon}", async (
                string codigoNivel, string codigoTrans, string codigoUne, string codigoMon, ISender sender) =>
            {
                var result = await sender.Send(
                    new GetAprobadoresPorCombinacionQuery(codigoNivel, codigoTrans, codigoUne, codigoMon));
                return Results.Ok(result);
            })
            .WithName("GetAprobadoresPorCombinacion")
            .RequireAuthorization("APROBADORESASIGNADOS.VIEW");

            group.MapPut("/{codigoNivel}/{codigoTrans}/{codigoUne}/{codigoMon}/{codigoTrabajador}", async (
                string codigoNivel, string codigoTrans, string codigoUne, string codigoMon, string codigoTrabajador,
                UpdateAprobadorAsignadoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoNivel, command.NivelCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoTrans, command.TipoTransaccionCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoUne, command.UnidadNegocioCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoMon, command.MonedaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoTrabajador, command.TrabajadorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateAprobadorAsignado")
            .RequireAuthorization("APROBADORESASIGNADOS.UPDATE");

            group.MapDelete("/{codigoNivel}/{codigoTrans}/{codigoUne}/{codigoMon}/{codigoTrabajador}", async (
                string codigoNivel, string codigoTrans, string codigoUne, string codigoMon, string codigoTrabajador,
                ISender sender) =>
            {
                await sender.Send(new DeleteAprobadorAsignadoCommand(
                    codigoNivel, codigoTrans, codigoUne, codigoMon, codigoTrabajador));
                return Results.NoContent();
            })
            .WithName("DeleteAprobadorAsignado")
            .RequireAuthorization("APROBADORESASIGNADOS.UPDATE");
        }
    }
}
