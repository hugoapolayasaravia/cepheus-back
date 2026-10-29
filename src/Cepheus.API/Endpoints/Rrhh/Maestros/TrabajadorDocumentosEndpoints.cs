using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.CreateTrabajadorDocumento;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.DeleteTrabajadorDocumento;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.GetTrabajadorDocumentoById;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.GetTrabajadorDocumentosByTrabajador;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.UpdateTrabajadorDocumento;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Maestros
{
    public static class TrabajadorDocumentosEndpoints
    {
        public static void MapTrabajadorDocumentosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/maestros/trabajadores/{codigoTrabajador}/documentos")
                .WithTags("Documentos de Trabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (string codigoTrabajador, CreateTrabajadorDocumentoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoTrabajador, command.TrabajadorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de trabajador de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/maestros/trabajadores/{codigoTrabajador}/documentos/{result.Id}", result);
            })
            .WithName("CreateTrabajadorDocumento")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapGet("/", async (string codigoTrabajador, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorDocumentosByTrabajadorQuery(codigoTrabajador));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorDocumentosByTrabajador")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapGet("/{id:int}", async (string codigoTrabajador, int id, ISender sender) =>
            {
                var result = await sender.Send(new GetTrabajadorDocumentoByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetTrabajadorDocumentoById")
            .RequireAuthorization("TRABAJADORES.VIEW");

            group.MapPut("/{id:int}", async (string codigoTrabajador, int id, UpdateTrabajadorDocumentoCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTrabajadorDocumento")
            .RequireAuthorization("TRABAJADORES.UPDATE");

            group.MapDelete("/{id:int}", async (string codigoTrabajador, int id, ISender sender) =>
            {
                await sender.Send(new DeleteTrabajadorDocumentoCommand(id));
                return Results.NoContent();
            })
            .WithName("DeleteTrabajadorDocumento")
            .RequireAuthorization("TRABAJADORES.UPDATE");
        }
    }
}