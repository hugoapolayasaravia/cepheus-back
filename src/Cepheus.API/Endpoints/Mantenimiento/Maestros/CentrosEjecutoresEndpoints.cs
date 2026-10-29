using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.CreateCentroEjecutor;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.GetCentroEjecutorByCode;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.GetCentrosEjecutoresPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.ToggleCentroEjecutorStatus;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.UpdateCentroEjecutor;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class CentrosEjecutoresEndpoints
    {
        public static void MapCentrosEjecutoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/centros-ejecutores")
                .WithTags("Centros Ejecutores (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCentroEjecutorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/centros-ejecutores/{result.Code}", result);
            })
            .WithName("CreateCentroEjecutor")
            .RequireAuthorization("CENTROSEJECUTORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCentrosEjecutoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCentrosEjecutoresPagedQueryString")
            .RequireAuthorization("CENTROSEJECUTORES.VIEW");

            group.MapPost("/paged/body", async (
                GetCentrosEjecutoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCentrosEjecutoresPagedBody")
            .RequireAuthorization("CENTROSEJECUTORES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCentroEjecutorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCentroEjecutorByCode")
            .RequireAuthorization("CENTROSEJECUTORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCentroEjecutorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCentroEjecutor")
            .RequireAuthorization("CENTROSEJECUTORES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCentroEjecutorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCentroEjecutorStatus")
            .RequireAuthorization("CENTROSEJECUTORES.UPDATE");
        }
    }
}
