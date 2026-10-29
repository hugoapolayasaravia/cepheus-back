using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.CreateSctrSalud;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.GetSctrSaludByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.GetSctrSaludPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.ToggleSctrSaludStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.UpdateSctrSalud;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class SctrSaludEndpoints
    {
        public static void MapSctrSaludEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/sctr-salud")
                .WithTags("SctrSalud (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSctrSaludCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/sctr-salud/{result.Code}", result);
            })
            .WithName("CreateSctrSalud")
            .RequireAuthorization("SCTRSALUD.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSctrSaludPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSctrSaludPagedQueryString")
            .RequireAuthorization("SCTRSALUD.VIEW");

            group.MapPost("/paged/body", async (
                GetSctrSaludPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSctrSaludPagedBody")
            .RequireAuthorization("SCTRSALUD.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSctrSaludByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSctrSaludByCode")
            .RequireAuthorization("SCTRSALUD.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSctrSaludCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSctrSalud")
            .RequireAuthorization("SCTRSALUD.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSctrSaludStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSctrSaludStatus")
            .RequireAuthorization("SCTRSALUD.UPDATE");
        }
    }
}