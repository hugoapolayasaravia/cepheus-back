using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.CreateTrabajadorLaboral;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.DeleteTrabajadorLaboral;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.GetTrabajadorLaboralsByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.UpdateTrabajadorLaboral;
using MediatR;
using System;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorLaboralsEndpoints
    {
        public static void MapTrabajadorLaboralsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-laborals")
                .WithTags("TrabajadorLaboral (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorLaboralCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-laborals/trabajador/{trabajadorCode}", result);
            })
            .WithName("CreateTrabajadorLaboral")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorLaboralsByTrabajadorQuery(trabajadorCode));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetTrabajadorLaboralByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorLaboralCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorLaboral")
            .RequireAuthorization("TRABAJADORES.UPDATE");


            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                try
                {
                    await sender.Send(new DeleteTrabajadorLaboralCommand(id));
                    return Results.NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(new
                    {
                        message = ex.Message
                    });
                }
            })
            .WithName("DeleteTrabajadorLaboral")
            .RequireAuthorization("TRABAJADORES.UPDATE");



        }
    }
}
