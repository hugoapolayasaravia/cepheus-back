using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.EliminarAsientoNotaIngreso;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientoNotaIngreso;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientosNotaIngresoMasivo;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GetNotasIngresoParaAsiento;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class AsientosNotaIngresoEndpoints
    {
        public static void MapAsientosNotaIngresoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/transacciones/asientos-nota-ingreso")
                .WithTags("Asientos contables - Nota de Ingresos")
                .RequireAuthorization();

            // Búsqueda de notas de ingreso a contabilizar (SP CO_Ingresos): planta, fechas y solo pendientes.
            group.MapGet("/pendientes", async ([AsParameters] GetNotasIngresoParaAsientoQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetNotasIngresoParaAsientoQueryString")
            .RequireAuthorization("ASIENTOSNOTAINGRESO.VIEW");

            group.MapPost("/pendientes/body", async (GetNotasIngresoParaAsientoQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetNotasIngresoParaAsientoBody")
            .RequireAuthorization("ASIENTOSNOTAINGRESO.VIEW");

            // Genera el documento de compra y el asiento de una nota de ingreso (o el asiento de una transferencia).
            group.MapPost("/generar", async (GenerarAsientoNotaIngresoCommand command, ISender sender) =>
                Results.Ok(await sender.Send(command)))
            .WithName("GenerarAsientoNotaIngreso")
            .RequireAuthorization("ASIENTOSNOTAINGRESO.CREATE");

            // Genera los asientos de todas las notas de ingreso pendientes de la planta en el rango de fechas.
            group.MapPost("/generar-masivo", async (GenerarAsientosNotaIngresoMasivoCommand command, ISender sender) =>
                Results.Ok(await sender.Send(command)))
            .WithName("GenerarAsientosNotaIngresoMasivo")
            .RequireAuthorization("ASIENTOSNOTAINGRESO.CREATE");

            // Elimina el asiento de la nota de ingreso (y su documento de compra si está pendiente y sin pagos).
            group.MapDelete("/{plantaCode}/{notaIngresoCode}", async (
                string plantaCode, string notaIngresoCode, ISender sender) =>
                Results.Ok(await sender.Send(new EliminarAsientoNotaIngresoCommand(plantaCode, notaIngresoCode))))
            .WithName("EliminarAsientoNotaIngreso")
            .RequireAuthorization("ASIENTOSNOTAINGRESO.DELETE");
        }
    }
}
