using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.CreateTrabajadorDomicilio;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.DeleteTrabajadorDomicilio;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.GetTrabajadorDomicilioById;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.GetTrabajadorDomiciliosByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.UpdateTrabajadorDomicilio;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorDomiciliosEndpoints
    {
        public static void MapTrabajadorDomiciliosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajadores/{codigoTrabajador}/domicilios")
                .WithTags("Domicilios de Trabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (string codigoTrabajador, CreateTrabajadorDomicilioCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoTrabajador, command.TrabajadorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de trabajador de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajadores/{codigoTrabajador}/domicilios/{result.Id}", result);
            })
            .WithName("CreateTrabajadorDomicilio")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapGet("/", async (string codigoTrabajador, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorDomiciliosByTrabajadorQuery(codigoTrabajador));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorDomiciliosByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapGet("/{id:int}", async (string codigoTrabajador, int id, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorDomicilioByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorDomicilioById")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:int}", async (string codigoTrabajador, int id, UpdateTrabajadorDomicilioCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorDomicilio")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:int}", async (string codigoTrabajador, int id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorDomicilioCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorDomicilio")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}