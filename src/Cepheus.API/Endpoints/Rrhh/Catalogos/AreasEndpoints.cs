using Cepheus.Application.Features.Rrhh.Catalogos.Areas.CreateArea;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.GetAreaByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.GetAreasPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.ToggleAreaStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.UpdateArea;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class AreasEndpoints
    {
        public static void MapAreasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/areas")
                .WithTags("Areas (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAreaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/areas/{result.Code}", result);
            })
            .WithName("CreateArea")
            .RequireAuthorization("AREAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAreasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAreasPagedQueryString")
            .RequireAuthorization("AREAS.VIEW");

            group.MapPost("/paged/body", async (
                GetAreasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAreasPagedBody")
            .RequireAuthorization("AREAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAreaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetAreaByCode")
            .RequireAuthorization("AREAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateAreaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateArea")
            .RequireAuthorization("AREAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleAreaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleAreaStatus")
            .RequireAuthorization("AREAS.UPDATE");
        }
    }
}