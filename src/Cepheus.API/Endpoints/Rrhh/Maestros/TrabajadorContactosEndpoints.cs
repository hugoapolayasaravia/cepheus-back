using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.CreateTrabajadorContacto;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.DeleteTrabajadorContacto;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.GetTrabajadorContactoById;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.GetTrabajadorContactosByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.UpdateTrabajadorContacto;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorContactosEndpoints
    {
        public static void MapTrabajadorContactosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajadores/{codigoTrabajador}/contactos")
                .WithTags("Contactos de Trabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (string codigoTrabajador, CreateTrabajadorContactoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoTrabajador, command.TrabajadorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de trabajador de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajadores/{codigoTrabajador}/contactos/{result.Id}", result);
            })
            .WithName("CreateTrabajadorContacto")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapGet("/", async (string codigoTrabajador, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorContactosByTrabajadorQuery(codigoTrabajador));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorContactosByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapGet("/{id:int}", async (string codigoTrabajador, int id, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorContactoByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorContactoById")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:int}", async (string codigoTrabajador, int id, UpdateTrabajadorContactoCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorContacto")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:int}", async (string codigoTrabajador, int id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorContactoCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorContacto")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}