using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.CreateSctrPension;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.GetSctrPensionByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.GetSctrPensionPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.ToggleSctrPensionStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.UpdateSctrPension;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class SctrPensionEndpoints
    {
        public static void MapSctrPensionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/sctr-pension")
                .WithTags("SctrPension (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSctrPensionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/sctr-pension/{result.Code}", result);
            })
            .WithName("CreateSctrPension")
            .RequireAuthorization("SCTRPENSION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSctrPensionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSctrPensionPagedQueryString")
            .RequireAuthorization("SCTRPENSION.VIEW");

            group.MapPost("/paged/body", async (
                GetSctrPensionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSctrPensionPagedBody")
            .RequireAuthorization("SCTRPENSION.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSctrPensionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSctrPensionByCode")
            .RequireAuthorization("SCTRPENSION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSctrPensionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSctrPension")
            .RequireAuthorization("SCTRPENSION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSctrPensionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSctrPensionStatus")
            .RequireAuthorization("SCTRPENSION.UPDATE");
        }
    }
}