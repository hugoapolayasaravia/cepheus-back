using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.CreateEspecialidad;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.GetEspecialidadByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.GetEspecialidadesPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.ToggleEspecialidadStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.UpdateEspecialidad;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class EspecialidadesEndpoints
    {
        public static void MapEspecialidadesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/especialidadestrabajador")
                .WithTags("Especialidades (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateEspecialidadTrabajadorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/especialidadestrabajador/{result.Code}", result);
            })
            .WithName("CreateEspecialidadTrabajador")
            .RequireAuthorization("ESPECIALIDADESTRABA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetEspecialidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadesTrabajadorPagedQueryString")
            .RequireAuthorization("ESPECIALIDADESTRABA.VIEW");

            group.MapPost("/paged/body", async (
                GetEspecialidadesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadesTrabajadorPagedBody")
            .RequireAuthorization("ESPECIALIDADESTRABA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetEspecialidadByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetEspecialidadTrabajadorByCode")
            .RequireAuthorization("ESPECIALIDADESTRABA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateEspecialidadTrabajadorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateEspecialidadTrabajador")
            .RequireAuthorization("ESPECIALIDADESTRABA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleEspecialidadStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleEspecialidadTrabajadorStatus")
            .RequireAuthorization("ESPECIALIDADESTRABA.UPDATE");
        }
    }
}