using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.CreateTrabajadorVacacion;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.GetTrabajadorVacacionsByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.UpdateTrabajadorVacacion;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.DeleteTrabajadorVacacion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorVacacionsEndpoints
    {
        public static void MapTrabajadorVacacionsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-vacacions")
                .WithTags("TrabajadorVacacion (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorVacacionCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-vacacions/{result.Id}", result);
            })
            .WithName("CreateTrabajadorVacacion")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorVacacionsByTrabajadorQuery(trabajadorCode));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorVacacionsByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorVacacionCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorVacacion")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorVacacionCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorVacacion")
            .RequireAuthorization("TRABAJADORES.UPDATE");


        }
    }
}
