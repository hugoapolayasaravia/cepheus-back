using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.CreateInspeccion;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.GetInspeccionByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.GetInspeccionesPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.ToggleInspeccionStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.UpdateInspeccion;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class InspeccionesEndpoints
    {
        public static void MapInspeccionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/inspecciones")
                .WithTags("Inspecciones (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateInspeccionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/inspecciones/{result.Code}", result);
            })
            .WithName("CreateInspeccion")
            .RequireAuthorization("INSPECCIONES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetInspeccionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetInspeccionesPagedQueryString")
            .RequireAuthorization("INSPECCIONES.VIEW");

            group.MapPost("/paged/body", async (
                GetInspeccionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetInspeccionesPagedBody")
            .RequireAuthorization("INSPECCIONES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetInspeccionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetInspeccionByCode")
            .RequireAuthorization("INSPECCIONES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateInspeccionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateInspeccion")
            .RequireAuthorization("INSPECCIONES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleInspeccionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleInspeccionStatus")
            .RequireAuthorization("INSPECCIONES.UPDATE");
        }
    }
}
