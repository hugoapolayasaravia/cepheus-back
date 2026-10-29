using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.CreateTrabajadorRemuneracion;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.DeleteTrabajadorContrato;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.GetTrabajadorRemuneracionsByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.UpdateTrabajadorRemuneracion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorRemuneracionsEndpoints
    {
        public static void MapTrabajadorRemuneracionsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-remuneracions")
                .WithTags("TrabajadorRemuneracion (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorRemuneracionCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-remuneracions/trabajador/{trabajadorCode}", result);
            })
            .WithName("CreateTrabajadorRemuneracion")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorRemuneracionsByTrabajadorQuery(trabajadorCode));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetTrabajadorRemuneracionByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorRemuneracionCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorRemuneracion")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorRemuneracionCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorRemuneracion")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}
