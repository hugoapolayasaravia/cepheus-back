using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.CreateActividad;
using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.GetActividadByCode;
using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.GetActividadesPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.ToggleActividadStatus;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class ActividadesEndpoints
    {
        public static void MapActividadesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/actividades")
                .WithTags("Actividades (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateActividadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/actividades/{result.Code}", result);
            })
            .WithName("CreateActividad")
            .RequireAuthorization("ACTIVIDADES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetActividadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetActividadesPagedQueryString")
            .RequireAuthorization("ACTIVIDADES.VIEW");

            group.MapPost("/paged/body", async (
                GetActividadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetActividadesPagedBody")
            .RequireAuthorization("ACTIVIDADES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetActividadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetActividadByCode")
            .RequireAuthorization("ACTIVIDADES.VIEW");

            // No hay PUT/Update: Code es derivado de VerboActividadCode +
            // ObjetoActividadCode, así que "editar" una Actividad
            // significaría crear una combinación distinta. Solo se permite
            // activar/desactivar.
            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleActividadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleActividadStatus")
            .RequireAuthorization("ACTIVIDADES.UPDATE");
        }
    }
}
