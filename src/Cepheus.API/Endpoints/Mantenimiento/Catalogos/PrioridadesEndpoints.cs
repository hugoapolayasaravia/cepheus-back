using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.CreatePrioridad;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.GetPrioridadByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.GetPrioridadesPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.TogglePrioridadStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.UpdatePrioridad;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class PrioridadesEndpoints
    {
        public static void MapPrioridadesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/prioridades")
                .WithTags("Prioridades (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePrioridadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/prioridades/{result.Code}", result);
            })
            .WithName("CreatePrioridad")
            .RequireAuthorization("PRIORIDADES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetPrioridadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPrioridadesPagedQueryString")
            .RequireAuthorization("PRIORIDADES.VIEW");

            group.MapPost("/paged/body", async (
                GetPrioridadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPrioridadesPagedBody")
            .RequireAuthorization("PRIORIDADES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetPrioridadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetPrioridadByCode")
            .RequireAuthorization("PRIORIDADES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdatePrioridadCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePrioridad")
            .RequireAuthorization("PRIORIDADES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new TogglePrioridadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("TogglePrioridadStatus")
            .RequireAuthorization("PRIORIDADES.UPDATE");
        }
    }
}
