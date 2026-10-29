using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.CreateTrabajadorFormacion;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.GetTrabajadorFormacionsByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.UpdateTrabajadorFormacion;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.DeleteTrabajadorFormacion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorFormacionsEndpoints
    {
        public static void MapTrabajadorFormacionsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-formacions")
                .WithTags("TrabajadorFormacion (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorFormacionCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-formacions/{result.Id}", result);
            })
            .WithName("CreateTrabajadorFormacion")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorFormacionsByTrabajadorQuery(trabajadorCode));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorFormacionsByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorFormacionCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorFormacion")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorFormacionCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorFormacion")
            .RequireAuthorization("TRABAJADORES.UPDATE");


        }
    }
}
