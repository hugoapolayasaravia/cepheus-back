using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.CreateSubCentroEjecutor;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.GetSubCentroEjecutorByCode;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.GetSubCentrosEjecutoresPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.ToggleSubCentroEjecutorStatus;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.UpdateSubCentroEjecutor;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class SubCentrosEjecutoresEndpoints
    {
        public static void MapSubCentrosEjecutoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/subcentros-ejecutores")
                .WithTags("Subcentros Ejecutores (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSubCentroEjecutorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/subcentros-ejecutores/{result.Code}", result);
            })
            .WithName("CreateSubCentroEjecutor")
            .RequireAuthorization("SUBCENTROSEJECUTORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSubCentrosEjecutoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubCentrosEjecutoresPagedQueryString")
            .RequireAuthorization("SUBCENTROSEJECUTORES.VIEW");

            group.MapPost("/paged/body", async (
                GetSubCentrosEjecutoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubCentrosEjecutoresPagedBody")
            .RequireAuthorization("SUBCENTROSEJECUTORES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSubCentroEjecutorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSubCentroEjecutorByCode")
            .RequireAuthorization("SUBCENTROSEJECUTORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSubCentroEjecutorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSubCentroEjecutor")
            .RequireAuthorization("SUBCENTROSEJECUTORES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSubCentroEjecutorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSubCentroEjecutorStatus")
            .RequireAuthorization("SUBCENTROSEJECUTORES.UPDATE");
        }
    }
}
