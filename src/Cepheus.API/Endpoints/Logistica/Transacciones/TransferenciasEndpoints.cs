using Cepheus.Application.Features.Logistica.Transacciones.Transferencias.GenerarTransferencia;
using Cepheus.Application.Features.Logistica.Transacciones.Transferencias.GetTransferenciasPendientes;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class TransferenciasEndpoints
{
    public static void MapTransferenciasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/transferencias")
            .WithTags("Transferencias entre almacenes (Logística)")
            .RequireAuthorization();

        // Vales de transferencia enviados a la planta y todavía no recibidos.
        group.MapGet("/pendientes", async (string plantaDestinoCode, ISender sender) =>
            Results.Ok(await sender.Send(new GetTransferenciasPendientesQuery(plantaDestinoCode))))
        .WithName("GetTransferenciasPendientes")
        .RequireAuthorization("TRANSFERENCIAS.VIEW");

        // Recibe el vale en el destino: genera la Nota de Ingreso y mueve el stock de ambas plantas.
        group.MapPost("/", async (GenerarTransferenciaCommand command, ISender sender) =>
        {
            var nota = await sender.Send(command);
            return Results.Created(
                $"/api/logistica/transacciones/notas-ingreso/{nota.PlantaCode}/{nota.Code}", nota);
        })
        .WithName("GenerarTransferencia")
        .RequireAuthorization("TRANSFERENCIAS.CREATE");
    }
}
