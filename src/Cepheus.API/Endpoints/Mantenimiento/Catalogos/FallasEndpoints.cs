using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.CreateFalla;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.GetFallaByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.GetFallasPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.ToggleFallaStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.UpdateFalla;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class FallasEndpoints
    {
        public static void MapFallasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/fallas")
                .WithTags("Fallas (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateFallaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/fallas/{result.Code}", result);
            })
            .WithName("CreateFalla")
            .RequireAuthorization("FALLAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetFallasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFallasPagedQueryString")
            .RequireAuthorization("FALLAS.VIEW");

            group.MapPost("/paged/body", async (
                GetFallasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFallasPagedBody")
            .RequireAuthorization("FALLAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetFallaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFallaByCode")
            .RequireAuthorization("FALLAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateFallaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFalla")
            .RequireAuthorization("FALLAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleFallaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFallaStatus")
            .RequireAuthorization("FALLAS.UPDATE");
        }
    }
}
