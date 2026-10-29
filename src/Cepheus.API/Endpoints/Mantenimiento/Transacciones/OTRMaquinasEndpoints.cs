using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.CreateOTRMaquina;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.DeleteOTRMaquina;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.GetOTRMaquinasByOrdenTrabajo;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.UpdateOTRMaquina;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones
{
    public static class OTRMaquinasEndpoints
    {
        public static void MapOTRMaquinasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/transacciones/ot-maquinas")
                .WithTags("OT - Máquinas (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOTRMaquinaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/mantenimiento/transacciones/ot-maquinas/{result.PlantaCode}/{result.OrdenTrabajoCode}/{result.MaquinaCode}",
                    result);
            })
            .WithName("CreateOTRMaquina")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");

            group.MapGet("/{codigoPlanta}/{codigoOrden}", async (
                string codigoPlanta, string codigoOrden, ISender sender) =>
            {
                var result = await sender.Send(new GetOTRMaquinasByOrdenTrabajoQuery(codigoPlanta, codigoOrden));
                return Results.Ok(result);
            })
            .WithName("GetOTRMaquinasByOrdenTrabajo")
            .RequireAuthorization("ORDENESTRABAJO.VIEW");

            group.MapPut("/{codigoPlanta}/{codigoOrden}/{codigoMaquina}", async (
                string codigoPlanta, string codigoOrden, string codigoMaquina,
                UpdateOTRMaquinaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoOrden, command.OrdenTrabajoCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoMaquina, command.MaquinaCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateOTRMaquina")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoOrden}/{codigoMaquina}", async (
                string codigoPlanta, string codigoOrden, string codigoMaquina, ISender sender) =>
            {
                await sender.Send(new DeleteOTRMaquinaCommand(codigoPlanta, codigoOrden, codigoMaquina));
                return Results.NoContent();
            })
            .WithName("DeleteOTRMaquina")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");
        }
    }
}
