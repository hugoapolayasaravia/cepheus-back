using Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.GetReporteCierre;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class ReporteCierreEndpoints
{
    public static void MapReporteCierreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/reporte-cierre")
            .WithTags("Reporte del Cierre (Logística)")
            .RequireAuthorization();

        // Saldos y costos de un período cerrado comparados con otro (por defecto el anterior).
        // origen: T (todos), N (nacional), I (importado). Familia y subfamilia por rango desde-hasta.
        group.MapGet("/", async (
            string plantaCode,
            string periodoCode,
            string? periodoComparacionCode,
            string? origen,
            string? familiaDesde,
            string? familiaHasta,
            string? subFamiliaDesde,
            string? subFamiliaHasta,
            string? articuloCode,
            bool? incluirSaldoCero,
            ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteCierreQuery(
                    plantaCode, periodoCode, periodoComparacionCode, origen,
                    familiaDesde, familiaHasta, subFamiliaDesde, subFamiliaHasta,
                    articuloCode, incluirSaldoCero ?? false))))
        .WithName("GetReporteCierre")
        .RequireAuthorization("REPORTECIERRE.VIEW");
    }
}
