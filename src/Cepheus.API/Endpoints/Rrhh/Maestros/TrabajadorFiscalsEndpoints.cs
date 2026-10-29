using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.CreateTrabajadorFiscal;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.DeleteTrabajadorFiscal;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.GetTrabajadorFiscalsByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.UpdateTrabajadorFiscal;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorFiscalsEndpoints
    {
        public static void MapTrabajadorFiscalsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-fiscals")
                .WithTags("TrabajadorFiscal (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorFiscalCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-fiscals/trabajador/{trabajadorCode}", result);
            })
            .WithName("CreateTrabajadorFiscal")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorFiscalsByTrabajadorQuery(trabajadorCode));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetTrabajadorFiscalByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorFiscalCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorFiscal")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorFiscalCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorFiscal")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}
