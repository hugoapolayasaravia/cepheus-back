using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.CreateRangoAprobacion;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.DeleteRangoAprobacion;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.GetRangoAprobacionByKey;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.GetRangosAprobacionPaginated;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.ResolverRangoAprobacion;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.UpdateRangoAprobacion;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class RangosAprobacionEndpoints
    {
        public static void MapRangosAprobacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/rangos-aprobacion")
                .WithTags("Rangos de Aprobación (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateRangoAprobacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/maestros/rangos-aprobacion/{result.NivelCode}/{result.TipoTransaccionCode}/{result.UnidadNegocioCode}/{result.MonedaCode}",
                    result);
            })
            .WithName("CreateRangoAprobacion")
            .RequireAuthorization("RANGOSAPROBACION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetRangosAprobacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRangosAprobacionPagedQueryString")
            .RequireAuthorization("RANGOSAPROBACION.VIEW");

            group.MapPost("/paged/body", async (
                GetRangosAprobacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRangosAprobacionPagedBody")
            .RequireAuthorization("RANGOSAPROBACION.VIEW");

            group.MapGet("/{codigoNivel}/{codigoTrans}/{codigoUne}/{codigoMon}", async (
                string codigoNivel, string codigoTrans, string codigoUne, string codigoMon, ISender sender) =>
            {
                var result = await sender.Send(
                    new GetRangoAprobacionByKeyQuery(codigoNivel, codigoTrans, codigoUne, codigoMon));
                return Results.Ok(result);
            })
            .WithName("GetRangoAprobacionByKey")
            .RequireAuthorization("RANGOSAPROBACION.VIEW");

            group.MapPut("/{codigoNivel}/{codigoTrans}/{codigoUne}/{codigoMon}", async (
                string codigoNivel, string codigoTrans, string codigoUne, string codigoMon,
                UpdateRangoAprobacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoNivel, command.NivelCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoTrans, command.TipoTransaccionCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoUne, command.UnidadNegocioCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoMon, command.MonedaCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateRangoAprobacion")
            .RequireAuthorization("RANGOSAPROBACION.UPDATE");

            group.MapDelete("/{codigoNivel}/{codigoTrans}/{codigoUne}/{codigoMon}", async (
                string codigoNivel, string codigoTrans, string codigoUne, string codigoMon, ISender sender) =>
            {
                await sender.Send(new DeleteRangoAprobacionCommand(codigoNivel, codigoTrans, codigoUne, codigoMon));
                return Results.NoContent();
            })
            .WithName("DeleteRangoAprobacion")
            .RequireAuthorization("RANGOSAPROBACION.UPDATE");

            // Resolución con herencia por jerarquía de UnidadNegocio.
            group.MapGet("/resolver", async (
                string codigoTrans, string codigoUne, string codigoMon, decimal monto, ISender sender) =>
            {
                var result = await sender.Send(
                    new ResolverRangoAprobacionQuery(codigoTrans, codigoUne, codigoMon, monto));
                return Results.Ok(result);
            })
            .WithName("ResolverRangoAprobacion")
            .RequireAuthorization("RANGOSAPROBACION.VIEW");
        }
    }
}
