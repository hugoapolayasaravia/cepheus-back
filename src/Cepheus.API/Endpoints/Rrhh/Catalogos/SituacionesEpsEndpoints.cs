using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.CreateSituacionEps;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.GetSituacionEpsByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.GetSituacionesEpsPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.ToggleSituacionEpsStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.UpdateSituacionEps;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class SituacionesEpsEndpoints
    {
        public static void MapSituacionesEpsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/situaciones-eps")
                .WithTags("SituacionesEps (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSituacionEpsCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/situaciones-eps/{result.Code}", result);
            })
            .WithName("CreateSituacionEps")
            .RequireAuthorization("SITUACIONESEPS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSituacionesEpsPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSituacionesEpsPagedQueryString")
            .RequireAuthorization("SITUACIONESEPS.VIEW");

            group.MapPost("/paged/body", async (
                GetSituacionesEpsPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSituacionesEpsPagedBody")
            .RequireAuthorization("SITUACIONESEPS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSituacionEpsByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSituacionEpsByCode")
            .RequireAuthorization("SITUACIONESEPS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSituacionEpsCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSituacionEps")
            .RequireAuthorization("SITUACIONESEPS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSituacionEpsStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSituacionEpsStatus")
            .RequireAuthorization("SITUACIONESEPS.UPDATE");
        }
    }
}