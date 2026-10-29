using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.CreateTrabajadorSeguro;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.DeleteTrabajadorSeguro;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.GetTrabajadorSegurosByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.UpdateTrabajadorSeguro;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorSegurosEndpoints
    {
        public static void MapTrabajadorSegurosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-seguros")
                .WithTags("TrabajadorSeguro (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorSeguroCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-seguros/trabajador/{trabajadorCode}", result);
            })
            .WithName("CreateTrabajadorSeguro")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorSegurosByTrabajadorQuery(trabajadorCode));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetTrabajadorSeguroByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorSeguroCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorSeguro")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorSeguroCommand(id));
                return Results.NoContent();
            })
                .WithName("DeleteTrabajadorSeguro")
                .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}
