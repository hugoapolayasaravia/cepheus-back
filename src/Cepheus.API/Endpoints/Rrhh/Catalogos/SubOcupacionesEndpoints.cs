using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.CreateSubOcupacion;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.GetSubOcupacionByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.GetSubOcupacionesPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.ToggleSubOcupacionStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.UpdateSubOcupacion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class SubOcupacionesEndpoints
    {
        public static void MapSubOcupacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/sub-ocupaciones")
                .WithTags("SubOcupaciones (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSubOcupacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/sub-ocupaciones/{result.Code}", result);
            })
            .WithName("CreateSubOcupacion")
            .RequireAuthorization("SUBOCUPACIONES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSubOcupacionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubOcupacionesPagedQueryString")
            .RequireAuthorization("SUBOCUPACIONES.VIEW");

            group.MapPost("/paged/body", async (
                GetSubOcupacionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubOcupacionesPagedBody")
            .RequireAuthorization("SUBOCUPACIONES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSubOcupacionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSubOcupacionByCode")
            .RequireAuthorization("SUBOCUPACIONES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSubOcupacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSubOcupacion")
            .RequireAuthorization("SUBOCUPACIONES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSubOcupacionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSubOcupacionStatus")
            .RequireAuthorization("SUBOCUPACIONES.UPDATE");
        }
    }
}