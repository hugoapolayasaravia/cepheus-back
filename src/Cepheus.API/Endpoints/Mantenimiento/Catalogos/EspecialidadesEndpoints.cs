using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.CreateEspecialidad;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.GetEspecialidadByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.GetEspecialidadesPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.ToggleEspecialidadStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.UpdateEspecialidad;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class EspecialidadesEndpoints
    {
        public static void MapEspecialidadesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/especialidades")
                .WithTags("Especialidades (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateEspecialidadCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/especialidades/{result.Code}", result);
            })
            .WithName("CreateEspecialidad")
            .RequireAuthorization("ESPECIALIDADES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetEspecialidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadesPagedQueryString")
            .RequireAuthorization("ESPECIALIDADES.VIEW");

            group.MapPost("/paged/body", async (
                GetEspecialidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadesPagedBody")
            .RequireAuthorization("ESPECIALIDADES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetEspecialidadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadByCode")
            .RequireAuthorization("ESPECIALIDADES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateEspecialidadCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateEspecialidad")
            .RequireAuthorization("ESPECIALIDADES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleEspecialidadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleEspecialidadStatus")
            .RequireAuthorization("ESPECIALIDADES.UPDATE");
        }
    }
}
