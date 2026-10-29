using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.CreateTrabajadorCuentaBancaria;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.GetTrabajadorCuentaBancariasByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.UpdateTrabajadorCuentaBancaria;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.DeleteTrabajadorCuentaBancaria;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorCuentaBancariasEndpoints
    {
        public static void MapTrabajadorCuentaBancariasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-cuenta-bancarias")
                .WithTags("TrabajadorCuentaBancaria (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorCuentaBancariaCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-cuenta-bancarias/{result.Id}", result);
            })
            .WithName("CreateTrabajadorCuentaBancaria")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorCuentaBancariasByTrabajadorQuery(trabajadorCode));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorCuentaBancariasByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorCuentaBancariaCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorCuentaBancaria")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorCuentaBancariaCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorCuentaBancaria")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}
