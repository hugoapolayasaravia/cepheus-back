using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.CreateOTResponsable;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.DeleteOTResponsable;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.GetOTResponsablesByOrdenTrabajo;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.UpdateOTResponsable;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones
{
    public static class OTResponsablesEndpoints
    {
        public static void MapOTResponsablesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/transacciones/ot-responsables")
                .WithTags("OT - Responsables (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOTResponsableCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/mantenimiento/transacciones/ot-responsables/{result.PlantaCode}/{result.OrdenTrabajoCode}", result);
            })
            .WithName("CreateOTResponsable")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");

            group.MapGet("/{codigoPlanta}/{codigoOrden}", async (
                string codigoPlanta, string codigoOrden, ISender sender) =>
            {
                var result = await sender.Send(new GetOTResponsablesByOrdenTrabajoQuery(codigoPlanta, codigoOrden));
                return Results.Ok(result);
            })
            .WithName("GetOTResponsablesByOrdenTrabajo")
            .RequireAuthorization("ORDENESTRABAJO.VIEW");

            group.MapPut("/{codigoPlanta}/{codigoOrden}/{codigoTrabajador}/{fechaProceso}", async (
                string codigoPlanta, string codigoOrden, string codigoTrabajador, DateTime fechaProceso,
                UpdateOTResponsableCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoOrden, command.OrdenTrabajoCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoTrabajador, command.TrabajadorCode, StringComparison.OrdinalIgnoreCase) ||
                    fechaProceso.Date != command.FechaProceso.Date)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateOTResponsable")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoOrden}/{codigoTrabajador}/{fechaProceso}", async (
                string codigoPlanta, string codigoOrden, string codigoTrabajador, DateTime fechaProceso, ISender sender) =>
            {
                await sender.Send(new DeleteOTResponsableCommand(codigoPlanta, codigoOrden, fechaProceso, codigoTrabajador));
                return Results.NoContent();
            })
            .WithName("DeleteOTResponsable")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");
        }
    }
}
