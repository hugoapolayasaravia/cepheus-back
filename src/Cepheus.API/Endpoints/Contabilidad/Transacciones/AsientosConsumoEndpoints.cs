using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.EliminarAsientoConsumo;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.GenerarAsientoConsumo;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.GetValesParaAsiento;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class AsientosConsumoEndpoints
    {
        public static void MapAsientosConsumoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/transacciones/asientos-consumo")
                .WithTags("Asientos contables - Consumo")
                .RequireAuthorization();

            // Búsqueda de líneas de vale de salida a contabilizar (SP CO_consultaconsumo): planta, fechas y solo pendientes.
            group.MapGet("/pendientes", async ([AsParameters] GetValesParaAsientoQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetValesParaAsientoQueryString")
            .RequireAuthorization("ASIENTOSCONSUMO.VIEW");

            group.MapPost("/pendientes/body", async (GetValesParaAsientoQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetValesParaAsientoBody")
            .RequireAuthorization("ASIENTOSCONSUMO.VIEW");

            // Genera UN asiento con todas las líneas de vale pendientes de la planta y el rango de fechas.
            group.MapPost("/generar", async (GenerarAsientoConsumoCommand command, ISender sender) =>
                Results.Ok(await sender.Send(command)))
            .WithName("GenerarAsientoConsumo")
            .RequireAuthorization("ASIENTOSCONSUMO.CREATE");

            // Elimina el asiento de consumo completo (código de 13 caracteres) y libera sus vales.
            group.MapDelete("/{asientoCodigo}", async (string asientoCodigo, ISender sender) =>
                Results.Ok(await sender.Send(new EliminarAsientoConsumoCommand(asientoCodigo))))
            .WithName("EliminarAsientoConsumo")
            .RequireAuthorization("ASIENTOSCONSUMO.DELETE");
        }
    }
}
