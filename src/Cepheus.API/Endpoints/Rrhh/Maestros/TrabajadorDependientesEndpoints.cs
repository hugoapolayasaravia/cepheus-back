using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.CreateTrabajadorDependiente;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.GetTrabajadorDependientesByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.UpdateTrabajadorDependiente;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.DeleteTrabajadorDependiente;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorDependientesEndpoints
    {
        public static void MapTrabajadorDependientesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-dependientes")
                .WithTags("TrabajadorDependiente (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorDependienteCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-dependientes/{result.Id}", result);
            })
            .WithName("CreateTrabajadorDependiente")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorDependientesByTrabajadorQuery(trabajadorCode));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorDependientesByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorDependienteCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorDependiente")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorDependienteCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorDependiente")
            .RequireAuthorization("TRABAJADORES.UPDATE");

        }
    }
}
