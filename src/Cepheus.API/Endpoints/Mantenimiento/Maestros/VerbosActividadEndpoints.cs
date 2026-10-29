using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.CreateVerboActividad;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.GetVerboActividadByCode;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.GetVerbosActividadPaginated;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.ToggleVerboActividadStatus;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.UpdateVerboActividad;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Maestros
{
    public static class VerbosActividadEndpoints
    {
        public static void MapVerbosActividadEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/maestros/verbos-actividad")
                .WithTags("Verbos de Actividad (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateVerboActividadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/maestros/verbos-actividad/{result.Code}", result);
            })
            .WithName("CreateVerboActividad")
            .RequireAuthorization("VERBOSACTIVIDAD.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetVerbosActividadPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetVerbosActividadPagedQueryString")
            .RequireAuthorization("VERBOSACTIVIDAD.VIEW");

            group.MapPost("/paged/body", async (
                GetVerbosActividadPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetVerbosActividadPagedBody")
            .RequireAuthorization("VERBOSACTIVIDAD.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetVerboActividadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetVerboActividadByCode")
            .RequireAuthorization("VERBOSACTIVIDAD.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateVerboActividadCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateVerboActividad")
            .RequireAuthorization("VERBOSACTIVIDAD.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleVerboActividadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleVerboActividadStatus")
            .RequireAuthorization("VERBOSACTIVIDAD.UPDATE");
        }
    }
}
