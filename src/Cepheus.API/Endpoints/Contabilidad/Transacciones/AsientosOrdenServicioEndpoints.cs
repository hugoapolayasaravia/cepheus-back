using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.EliminarAsientoOrdenServicio;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.GenerarAsientoOrdenServicio;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.GetOrdenesServicioParaAsiento;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class AsientosOrdenServicioEndpoints
    {
        public static void MapAsientosOrdenServicioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/transacciones/asientos-orden-servicio")
                .WithTags("Asientos contables - Órdenes de Servicio")
                .RequireAuthorization();

            // Búsqueda de órdenes de servicio a contabilizar (SP CO_OrdenServicio): planta, fechas y solo pendientes.
            group.MapGet("/pendientes", async ([AsParameters] GetOrdenesServicioParaAsientoQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetOrdenesServicioParaAsientoQueryString")
            .RequireAuthorization("ASIENTOSORDENSERVICIO.VIEW");

            group.MapPost("/pendientes/body", async (GetOrdenesServicioParaAsientoQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetOrdenesServicioParaAsientoBody")
            .RequireAuthorization("ASIENTOSORDENSERVICIO.VIEW");

            // Genera el documento de compra y el asiento de una orden de servicio (solo individual, como el legacy).
            group.MapPost("/generar", async (GenerarAsientoOrdenServicioCommand command, ISender sender) =>
                Results.Ok(await sender.Send(command)))
            .WithName("GenerarAsientoOrdenServicio")
            .RequireAuthorization("ASIENTOSORDENSERVICIO.CREATE");

            // Elimina el asiento de la orden de servicio (y su documento de compra si está pendiente y sin pagos).
            group.MapDelete("/{plantaCode}/{ordenServicioCode}", async (
                string plantaCode, string ordenServicioCode, ISender sender) =>
                Results.Ok(await sender.Send(new EliminarAsientoOrdenServicioCommand(plantaCode, ordenServicioCode))))
            .WithName("EliminarAsientoOrdenServicio")
            .RequireAuthorization("ASIENTOSORDENSERVICIO.DELETE");
        }
    }
}
