using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GenerarAperturaKardex;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GetCierreIncidencias;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GetCierreMensualEstado;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.ProcesarCierreMensual;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.ReabrirCierreMensual;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class CierresMensualesEndpoints
{
    public static void MapCierresMensualesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/cierres-mensuales")
            .WithTags("Cierre Mensual de Artículos (Logística)")
            .RequireAuthorization();

        // Datos para la pantalla: último cierre, próximo período, si se puede procesar.
        group.MapGet("/{plantaCode}/estado", async (string plantaCode, ISender sender) =>
            Results.Ok(await sender.Send(new GetCierreMensualEstadoQuery(plantaCode))))
        .WithName("GetCierreMensualEstado")
        .RequireAuthorization("CIERREMENSUAL.VIEW");

        // Procesa el cierre. PeriodoCode solo es obligatorio en el primer cierre de la planta.
        group.MapPost("/", async (ProcesarCierreMensualCommand command, ISender sender) =>
            Results.Ok(await sender.Send(command)))
        .WithName("ProcesarCierreMensual")
        .RequireAuthorization("CIERREMENSUAL.CREATE");

        // Apertura del Kardex (una vez, antes del primer cierre): reemplaza la carga de stock iniciales del legacy.
        group.MapPost("/{plantaCode}/apertura-kardex",
            async (string plantaCode, GenerarAperturaKardexRequest? body, ISender sender) =>
                Results.Ok(await sender.Send(new GenerarAperturaKardexCommand(plantaCode, body?.FechaApertura))))
        .WithName("GenerarAperturaKardex")
        .RequireAuthorization("CIERREMENSUAL.CREATE");

        // Incidencias del cierre (reemplaza la tabla ERRORES del legacy).
        group.MapGet("/{plantaCode}/{periodCode}/incidencias",
            async (string plantaCode, string periodCode, string? tipo, ISender sender) =>
                Results.Ok(await sender.Send(new GetCierreIncidenciasQuery(plantaCode, periodCode, tipo))))
        .WithName("GetCierreIncidencias")
        .RequireAuthorization("CIERREMENSUAL.VIEW");

        // Reabre el último período cerrado.
        group.MapDelete("/{plantaCode}/ultimo", async (string plantaCode, ISender sender) =>
            Results.Ok(await sender.Send(new ReabrirCierreMensualCommand(plantaCode))))
        .WithName("ReabrirCierreMensual")
        .RequireAuthorization("CIERREMENSUAL.DELETE");
    }
}
public sealed record GenerarAperturaKardexRequest(DateTime? FechaApertura);
