using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.CreateCargo;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.GetCargoByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.GetCargosPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.ToggleCargoStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.UpdateCargo;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class CargosEndpoints
    {
        public static void MapCargosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/cargos")
                .WithTags("Cargos (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCargoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/cargos/{result.Code}", result);
            })
            .WithName("CreateCargo")
            .RequireAuthorization("CARGOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCargosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCargosPagedQueryString")
            .RequireAuthorization("CARGOS.VIEW");

            group.MapPost("/paged/body", async (
                GetCargosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCargosPagedBody")
            .RequireAuthorization("CARGOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCargoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCargoByCode")
            .RequireAuthorization("CARGOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCargoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCargo")
            .RequireAuthorization("CARGOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCargoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCargoStatus")
            .RequireAuthorization("CARGOS.UPDATE");
        }
    }
}