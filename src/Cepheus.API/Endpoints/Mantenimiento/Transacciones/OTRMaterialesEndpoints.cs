using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.CreateOTRMaterial;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.DeleteOTRMaterial;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.GetOTRMaterialesByOrdenTrabajo;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.UpdateOTRMaterial;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Transacciones
{
    public static class OTRMaterialesEndpoints
    {
        public static void MapOTRMaterialesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/transacciones/ot-materiales")
                .WithTags("OT - Materiales (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOTRMaterialCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/mantenimiento/transacciones/ot-materiales/{result.PlantaCode}/{result.OrdenTrabajoCode}", result);
            })
            .WithName("CreateOTRMaterial")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");

            group.MapGet("/{codigoPlanta}/{codigoOrden}", async (
                string codigoPlanta, string codigoOrden, ISender sender) =>
            {
                var result = await sender.Send(new GetOTRMaterialesByOrdenTrabajoQuery(codigoPlanta, codigoOrden));
                return Results.Ok(result);
            })
            .WithName("GetOTRMaterialesByOrdenTrabajo")
            .RequireAuthorization("ORDENESTRABAJO.VIEW");

            group.MapPut("/{codigoPlanta}/{codigoOrden}/{codigoArticulo}/{fechaProceso}", async (
                string codigoPlanta, string codigoOrden, string codigoArticulo, DateTime fechaProceso,
                UpdateOTRMaterialCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoOrden, command.OrdenTrabajoCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoArticulo, command.ArticuloCode, StringComparison.OrdinalIgnoreCase) ||
                    fechaProceso.Date != command.FechaProceso.Date)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateOTRMaterial")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoOrden}/{codigoArticulo}/{fechaProceso}", async (
                string codigoPlanta, string codigoOrden, string codigoArticulo, DateTime fechaProceso, ISender sender) =>
            {
                await sender.Send(new DeleteOTRMaterialCommand(codigoPlanta, codigoOrden, fechaProceso, codigoArticulo));
                return Results.NoContent();
            })
            .WithName("DeleteOTRMaterial")
            .RequireAuthorization("ORDENESTRABAJO.UPDATE");
        }
    }
}
