using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.CreateActividadMantenimiento;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.DeleteActividadMantenimiento;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.GetActividadesMantenimientoPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.UpdateActividadMantenimiento;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class ActividadesMantenimientoEndpoints
    {
        public static void MapActividadesMantenimientoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/actividades-mantenimiento")
                .WithTags("Actividades de Mantenimiento (prioridad y cobertura)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateActividadMantenimientoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/mantenimiento/maestros/actividades-mantenimiento/{result.ActividadCode}/{result.ActividadCubiertaCode}",
                    result);
            })
            .WithName("CreateActividadMantenimiento")
            .RequireAuthorization("ACTIVIDADESMANTENIMIENTO.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetActividadesMantenimientoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetActividadesMantenimientoPagedQueryString")
            .RequireAuthorization("ACTIVIDADESMANTENIMIENTO.VIEW");

            group.MapPost("/paged/body", async (GetActividadesMantenimientoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetActividadesMantenimientoPagedBody")
            .RequireAuthorization("ACTIVIDADESMANTENIMIENTO.VIEW");

            group.MapPut("/{actividadCode}/{actividadCubiertaCode}", async (
                string actividadCode, string actividadCubiertaCode,
                UpdateActividadMantenimientoCommand command, ISender sender) =>
            {
                if (!string.Equals(actividadCode, command.ActividadCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(actividadCubiertaCode, command.ActividadCubiertaCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateActividadMantenimiento")
            .RequireAuthorization("ACTIVIDADESMANTENIMIENTO.UPDATE");

            group.MapDelete("/{actividadCode}/{actividadCubiertaCode}", async (
                string actividadCode, string actividadCubiertaCode, ISender sender) =>
            {
                await sender.Send(new DeleteActividadMantenimientoCommand(actividadCode, actividadCubiertaCode));
                return Results.NoContent();
            })
            .WithName("DeleteActividadMantenimiento")
            .RequireAuthorization("ACTIVIDADESMANTENIMIENTO.DELETE");
        }
    }
}
