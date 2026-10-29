using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.CreateTrabajadorContable;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.GetTrabajadorContablesByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.UpdateTrabajadorContable;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.DeleteTrabajadorContable;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorContablesEndpoints
    {
        public static void MapTrabajadorContablesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-contables")
                .WithTags("TrabajadorContable (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorContableCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-contables/{result.Id}", result);
            })
            .WithName("CreateTrabajadorContable")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorContablesByTrabajadorQuery(trabajadorCode));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorContablesByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorContableCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorContable")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorContableCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorContable")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}
