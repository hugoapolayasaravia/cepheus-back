using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.CreateTrabajadorAntecedente;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.DeleteTrabajadorAntecedente;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.GetTrabajadorAntecedentesByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.UpdateTrabajadorAntecedente;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorAntecedentesEndpoints
    {
        public static void MapTrabajadorAntecedentesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajador-antecedentes")
                .WithTags("TrabajadorAntecedente (RRHH)")
                .RequireAuthorization();

            group.MapPost("/trabajador/{trabajadorCode}", async (string trabajadorCode, CreateTrabajadorAntecedenteCommand bodyCommand, ISender sender) =>
            {
                var command = bodyCommand with { TrabajadorCode = trabajadorCode };
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajador-antecedentes/trabajador/{trabajadorCode}", result);
            })
            .WithName("CreateTrabajadorAntecedente")
            .RequireAuthorization("TRABAJADORES.CREATE");

            group.MapGet("/trabajador/{trabajadorCode}", async (string trabajadorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorAntecedentesByTrabajadorQuery(trabajadorCode));
                return result is null ? Results.NotFound() : Results.Ok(result);
            })
            .WithName("GetTrabajadorAntecedenteByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateTrabajadorAntecedenteCommand bodyCommand, ISender sender) =>
            {
                if (id != bodyCommand.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(bodyCommand);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorAntecedente")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:long}", async (long id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorAntecedenteCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorAntecedente")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}
