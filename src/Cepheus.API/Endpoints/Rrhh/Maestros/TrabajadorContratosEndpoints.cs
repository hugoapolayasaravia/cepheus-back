using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.CreateTrabajadorContrato;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.DeleteTrabajadorContrato;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.GetTrabajadorContratosByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.UpdateTrabajadorContrato;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorContratosEndpoints
    {
        public static void MapTrabajadorContratosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-contratos")
                .WithTags("TrabajadorContrato (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorContratoCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-contratos/{result.Id}", result);
            })
            .WithName("CreateTrabajadorContrato")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorContratosByTrabajadorQuery(trabajadorCode));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorContratosByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorContratoCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorContrato")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorContratoCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorContrato")
            .RequireAuthorization("TRABAJADORES.UPDATE");   
        }
    }
}
