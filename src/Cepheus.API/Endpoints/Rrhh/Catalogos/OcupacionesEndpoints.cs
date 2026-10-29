using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.CreateOcupacion;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.GetOcupacionByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.GetOcupacionesPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.ToggleOcupacionStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.UpdateOcupacion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class OcupacionesEndpoints
    {
        public static void MapOcupacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/ocupaciones")
                .WithTags("Ocupaciones (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOcupacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/ocupaciones/{result.Code}", result);
            })
            .WithName("CreateOcupacion")
            .RequireAuthorization("OCUPACIONES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetOcupacionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOcupacionesPagedQueryString")
            .RequireAuthorization("OCUPACIONES.VIEW");

            group.MapPost("/paged/body", async (
                GetOcupacionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOcupacionesPagedBody")
            .RequireAuthorization("OCUPACIONES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetOcupacionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetOcupacionByCode")
            .RequireAuthorization("OCUPACIONES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateOcupacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateOcupacion")
            .RequireAuthorization("OCUPACIONES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleOcupacionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleOcupacionStatus")
            .RequireAuthorization("OCUPACIONES.UPDATE");
        }
    }
}