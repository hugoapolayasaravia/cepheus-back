using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.CreateEquipo;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.GetEquipoByCode;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.GetEquiposPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.ToggleEquipoStatus;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.UpdateEquipo;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class EquiposEndpoints
    {
        public static void MapEquiposEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/equipos")
                .WithTags("Equipos (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateEquipoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/equipos/{result.Code}", result);
            })
            .WithName("CreateEquipo")
            .RequireAuthorization("EQUIPOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetEquiposPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEquiposPagedQueryString")
            .RequireAuthorization("EQUIPOS.VIEW");

            group.MapPost("/paged/body", async (
                GetEquiposPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEquiposPagedBody")
            .RequireAuthorization("EQUIPOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetEquipoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetEquipoByCode")
            .RequireAuthorization("EQUIPOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateEquipoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateEquipo")
            .RequireAuthorization("EQUIPOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleEquipoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleEquipoStatus")
            .RequireAuthorization("EQUIPOS.UPDATE");
        }
    }
}
