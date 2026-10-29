using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.AnulateNotaIngreso;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.CreateNotaIngreso;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GenerarValeDesdeNotaIngreso;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoByCode;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoReferencia;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoValeStock;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresosPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetOrdenCompraPendiente;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.UpdateNotaIngreso;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class NotaIngresosEndpoints
{
    public static void MapNotaIngresosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/notas-ingreso")
            .WithTags("Notas de Ingreso (Logística)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateNotaIngresoCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created(
                $"/api/logistica/transacciones/notas-ingreso/{result.PlantaCode}/{result.Code}", result);
        })
        .WithName("CreateNotaIngreso")
        .RequireAuthorization("NOTASINGRESO.CREATE");

        // Listado: parámetros literales de Logi_sp_Listado_MNotaIngresos.
        group.MapGet("/paged", async ([AsParameters] GetNotaIngresosPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetNotaIngresosPagedQueryString")
        .RequireAuthorization("NOTASINGRESO.VIEW");

        group.MapPost("/paged/body", async (GetNotaIngresosPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetNotaIngresosPagedBody")
        .RequireAuthorization("NOTASINGRESO.VIEW");

        // Datos de la OC para recibirla (pendiente por artículo / pedido).
        group.MapGet("/ordenes-compra/{plantaCode}/{ordenCompraCode}/pendiente",
            async (string plantaCode, string ordenCompraCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetOrdenCompraPendienteQuery(plantaCode, ordenCompraCode))))
        .WithName("GetOrdenCompraPendienteNotaIngreso")
        .RequireAuthorization("NOTASINGRESO.VIEW");

        // Nota de Ingreso del documento que referencia una Nota de Crédito.
        group.MapGet("/referencia",
            async (string plantaCode, string comprobantePagoReferenciaCode, string proveedorCode, string numeroDocumento, ISender sender) =>
                Results.Ok(await sender.Send(new GetNotaIngresoReferenciaQuery(
                    plantaCode, comprobantePagoReferenciaCode, proveedorCode, numeroDocumento))))
        .WithName("GetNotaIngresoReferencia")
        .RequireAuthorization("NOTASINGRESO.VIEW");

        group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new GetNotaIngresoByCodeQuery(codigoPlanta, codigo))))
        .WithName("GetNotaIngresoByCode")
        .RequireAuthorization("NOTASINGRESO.VIEW");

        group.MapPut("/{codigoPlanta}/{codigo}", async (
            string codigoPlanta, string codigo, UpdateNotaIngresoRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateNotaIngresoCommand(
                codigoPlanta, codigo,
                body.ComprobantePagoCode, body.NumeroDocumento, body.NumeroGuia,
                body.FechaEmision, body.FechaRecepcion,
                body.Igv, body.NoGravable, body.Renta, body.Fonavi, body.Servicio, body.IgvExterior))))
        .WithName("UpdateNotaIngreso")
        .RequireAuthorization("NOTASINGRESO.UPDATE");

        // Botón "Vales": líneas de la nota que dejarían saldo negativo al generar el vale (lista vacía = se puede generar).
        group.MapGet("/{codigoPlanta}/{codigo}/vale/stock-insuficiente", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new GetNotaIngresoValeStockQuery(codigoPlanta, codigo))))
        .WithName("GetNotaIngresoValeStock")
        .RequireAuthorization("NOTASINGRESO.VIEW");

        // Botón "Vales": genera un Vale de Salida tipo Transferencia con las líneas de la nota (el stock no se mueve aquí).
        group.MapPost("/{codigoPlanta}/{codigo}/vale", async (
            string codigoPlanta, string codigo, GenerarValeDesdeNotaIngresoRequest body, ISender sender) =>
        {
            var vale = await sender.Send(new GenerarValeDesdeNotaIngresoCommand(
                codigoPlanta, codigo, body.FechaEntrega, body.SubCentroCostoCode,
                body.TrabajadorCode, body.UnidadNegocioCode, body.PlantaAfectadaCode));

            return Results.Created($"/api/logistica/transacciones/vales/{vale.PlantaCode}/{vale.Code}", vale);
        })
        .WithName("GenerarValeDesdeNotaIngreso")
        .RequireAuthorization("NOTASINGRESO.UPDATE");

        // Anulación (revierte stock y lo entregado en la OC). Mismo permiso UPDATE que OC / Pedidos.
        group.MapDelete("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new AnulateNotaIngresoCommand(codigoPlanta, codigo))))
        .WithName("AnulateNotaIngreso")
        .RequireAuthorization("NOTASINGRESO.UPDATE");
    }
}
public sealed record GenerarValeDesdeNotaIngresoRequest(
    DateTime FechaEntrega,
    string SubCentroCostoCode,
    string TrabajadorCode,
    string UnidadNegocioCode,
    string PlantaAfectadaCode);
