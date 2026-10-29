using Cepheus.Application.Features.Logistica.Reportes.Ajustes.GetReporteAjustesInventario;
using Cepheus.Application.Features.Logistica.Reportes.Compras.GetReporteCompras;
using Cepheus.Application.Features.Logistica.Reportes.ComprasAdm.GetReporteComprasAdm;
using Cepheus.Application.Features.Logistica.Reportes.Consumos.GetReporteConsumosProcesados;
using Cepheus.Application.Features.Logistica.Reportes.ConsumosAnexos.GetReporteConsumosAnexos;
using Cepheus.Application.Features.Logistica.Reportes.ConsumosCentroCostoArticulos.GetReporteConsumosCentroCostoArticulos;
using Cepheus.Application.Features.Logistica.Reportes.ConsumosFrecuencia.GetReporteConsumosFrecuencia;
using Cepheus.Application.Features.Logistica.Reportes.InventarioPermanente.GetReporteInventarioPermanenteValorizado;
using Cepheus.Application.Features.Logistica.Reportes.Pedidos.GetReportePedidosDetalle;
using Cepheus.Application.Features.Logistica.Reportes.SeguimientoOrdenCompra.GetReporteSeguimientoOrdenCompra;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Reportes;

public static class ReportesLogisticaEndpoints
{
    public static void MapReportesLogisticaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/reportes")
            .WithTags("Reportes (Logística)")
            .RequireAuthorization();

        // Compras: Notas de Ingreso y/o Órdenes de Servicio procesadas, en soles.
        // atencion: T (todos), N (Notas de Ingreso), S (Órdenes de Servicio).
        group.MapGet("/compras", async (
            string plantaCode, DateTime fechaDesde, DateTime fechaHasta,
            string? atencion, string? proveedorCode, string? comprobanteCode,
            string? ordenarPor, bool? descendente, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteComprasQuery(
                    plantaCode, fechaDesde, fechaHasta, atencion, proveedorCode, comprobanteCode,
                    ordenarPor, descendente ?? false))))
        .WithName("GetReporteCompras")
        .RequireAuthorization("REPORTECOMPRAS.VIEW");

        // Consumos procesados: Vales de Salida y salidas por Orden de Servicio.
        // tipoFecha: E (entrega), S (proceso), A (aprobación). fuente: T, N (vales), S (órdenes de servicio).
        group.MapGet("/consumos-procesados", async (
            string? plantaCode, DateTime fechaDesde, DateTime fechaHasta, string? tipoFecha,
            string? centroCostoCode, string? subCentroCostoCode, string? tipoValeCode, string? fuente,
            string? plantaAfectadaCode, string? familiaCode, string? subFamiliaCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteConsumosProcesadosQuery(
                    plantaCode, fechaDesde, fechaHasta, tipoFecha, centroCostoCode, subCentroCostoCode,
                    tipoValeCode, fuente, plantaAfectadaCode, familiaCode, subFamiliaCode))))
        .WithName("GetReporteConsumosProcesados")
        .RequireAuthorization("REPORTECONSUMOS.VIEW");

        // Consumo por centro de costo, subcentro y artículo (reemplaza los dos reportes legacy duplicados).
        group.MapGet("/consumos-centro-costo-articulos", async (
            string? plantaCode, DateTime fechaDesde, DateTime fechaHasta, string? tipoFecha,
            string? centroCostoCode, string? subCentroCostoCode, string? articuloCode,
            string? familiaCode, string? subFamiliaCode, string? trabajadorCode, string? tipoValeCode,
            int? estadoDetalleCode, int? estadoCode, string? plantaAfectadaCode, string? tipoOrdenCode,
            string? fuente, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteConsumosCentroCostoArticulosQuery(
                    plantaCode, fechaDesde, fechaHasta, tipoFecha, centroCostoCode, subCentroCostoCode,
                    articuloCode, familiaCode, subFamiliaCode, trabajadorCode, tipoValeCode,
                    estadoDetalleCode, estadoCode, plantaAfectadaCode, tipoOrdenCode, fuente))))
        .WithName("GetReporteConsumosCentroCostoArticulos")
        .RequireAuthorization("REPORTECONSUMOSART.VIEW");

        // Orden de Pedidos con detalle.
        group.MapGet("/pedidos", async (
            string? plantaCode, string? subCentroCostoCode, int? estadoCode,
            DateTime fechaDesde, DateTime fechaHasta, string? usuario, string? tipoPedidoCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetReportePedidosDetalleQuery(
                    plantaCode, subCentroCostoCode, estadoCode, fechaDesde, fechaHasta, usuario, tipoPedidoCode))))
        .WithName("GetReportePedidosDetalle")
        .RequireAuthorization("REPORTEPEDIDOS.VIEW");

        // Ajustes de Inventario procesados. tipo: T, Sobrante, Faltante.
        group.MapGet("/ajustes-inventario", async (
            string plantaCode, DateTime fechaDesde, DateTime fechaHasta, string? tipo, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteAjustesInventarioQuery(
                    plantaCode, fechaDesde, fechaHasta, tipo))))
        .WithName("GetReporteAjustesInventario")
        .RequireAuthorization("REPORTEAJUSTES.VIEW");
        // Compras Adm: Órdenes de Servicio con fecha de pago (planta OF por defecto).
        group.MapGet("/compras-adm", async (
            string? plantaCode, DateTime fechaDesde, DateTime fechaHasta, string? proveedorCode,
            string? comprobanteCode, string? compradorCode, int? estadoCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteComprasAdmQuery(
                    plantaCode, fechaDesde, fechaHasta, proveedorCode, comprobanteCode, compradorCode, estadoCode))))
        .WithName("GetReporteComprasAdm")
        .RequireAuthorization("REPORTECOMPRASADM.VIEW");

        // Seguimiento de Orden de Compra: pedido de origen, notas de ingreso y stock por artículo.
        group.MapGet("/seguimiento-orden-compra", async (
            string plantaCode, string? ordenCompraCode, DateTime? fechaDesde, DateTime? fechaHasta,
            string? proveedorCode, string? articuloCode, string? familiaCode, string? subFamiliaCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteSeguimientoOrdenCompraQuery(
                    plantaCode, ordenCompraCode, fechaDesde, fechaHasta, proveedorCode, articuloCode,
                    familiaCode, subFamiliaCode))))
        .WithName("GetReporteSeguimientoOrdenCompra")
        .RequireAuthorization("REPORTESEGUIMIENTOOC.VIEW");

        // Consumos por frecuencia: cantidad y precio promedio por mes y artículo.
        group.MapGet("/consumos-frecuencia", async (
            string plantaCode, DateTime fechaDesde, DateTime fechaHasta,
            string? familiaCode, string? subFamiliaCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteConsumosFrecuenciaQuery(
                    plantaCode, fechaDesde, fechaHasta, familiaCode, subFamiliaCode))))
        .WithName("GetReporteConsumosFrecuencia")
        .RequireAuthorization("REPORTECONSUMOSFRE.VIEW");

        // Inventario permanente valorizado (Kardex de todos los artículos). resumen=true: un renglón por artículo.
        group.MapGet("/inventario-permanente-valorizado", async (
            string plantaCode, DateTime fechaDesde, DateTime fechaHasta, string? familiaCode,
            string? familiaHastaCode, string? subFamiliaCode, string? subFamilia2Code, bool? resumen, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteInventarioPermanenteValorizadoQuery(
                    plantaCode, fechaDesde, fechaHasta, familiaCode, familiaHastaCode,
                    subFamiliaCode, subFamilia2Code, resumen ?? false))))
        .WithName("GetReporteInventarioPermanenteValorizado")
        .RequireAuthorization("REPORTEINVENTARIOPER.VIEW");

        // Consumos x Anexos (consumo gerencial): cuenta contable y anexo por subcentro + subfamilia.
        group.MapGet("/consumos-anexos", async (
            string plantaCode, DateTime fechaDesde, DateTime fechaHasta, string? accountingAccountCode,
            string? accountingAttachmentTypeCode, string? accountingAttachmentCode, string? centroCostoCode,
            string? subCentroCostoCode, string? plantaAfectadaCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetReporteConsumosAnexosQuery(
                    plantaCode, fechaDesde, fechaHasta, accountingAccountCode, accountingAttachmentTypeCode,
                    accountingAttachmentCode, centroCostoCode, subCentroCostoCode, plantaAfectadaCode))))
        .WithName("GetReporteConsumosAnexos")
        .RequireAuthorization("REPORTECONSUMOSANE.VIEW");

    }
}
