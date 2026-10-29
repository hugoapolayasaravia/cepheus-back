using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.AnularHojaCorte;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CopiarHojaCorte;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CreateHojaCorte;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.GetHojaCorteByCode;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.GetHojasCortePaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.CreateHojaCorteDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.DeleteHojaCorteDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.UpdateHojaCorteDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.ReporteHojaCorte;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.UpdateHojaCorte;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones;

public static class HojasCorteEndpoints
{
    public static void MapHojasCorteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/facturacion/transacciones/hojas-corte")
            .WithTags("Hojas de Corte (Facturación)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateHojaCorteCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            return Results.Created(
                $"/api/facturacion/transacciones/hojas-corte/{result.Year}/{result.Code}",
                result);
        })
        .WithName("CreateHojaCorte")
        .RequireAuthorization("HOJASCORTE.CREATE");

        // Búsqueda: parámetros literales de ges_sp_bus_hoja_corte.
        group.MapGet("/paged", async ([AsParameters] GetHojasCortePaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetHojasCortePagedQueryString")
        .RequireAuthorization("HOJASCORTE.VIEW");

        group.MapPost("/paged/body", async (GetHojasCortePaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetHojasCortePagedBody")
        .RequireAuthorization("HOJASCORTE.VIEW");

        // Reporte de Hoja de Corte: parámetros literales de ges_sp_rpt_hoja_corte (sin paginar; una fila por línea de detalle).
        group.MapGet("/reporte", async ([AsParameters] ReporteHojaCorteQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("ReporteHojaCorte")
        .RequireAuthorization("HOJASCORTE.VIEW");

        group.MapGet("/{year}/{code}", async (string year, string code, ISender sender) =>
            Results.Ok(await sender.Send(new GetHojaCorteByCodeQuery(year, code))))
        .WithName("GetHojaCorteByCode")
        .RequireAuthorization("HOJASCORTE.VIEW");

        group.MapPut("/{year}/{code}", async (
            string year,
            string code,
            UpdateHojaCorteRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new UpdateHojaCorteCommand(year, code, body))))
        .WithName("UpdateHojaCorte")
        .RequireAuthorization("HOJASCORTE.UPDATE");

        // Copiar: la copia nace Pendiente con la hoja de origen en ParentYear/ParentCode.
        group.MapPost("/{year}/{code}/copiar", async (string year, string code, ISender sender) =>
        {
            var result = await sender.Send(new CopiarHojaCorteCommand(year, code));

            return Results.Created(
                $"/api/facturacion/transacciones/hojas-corte/{result.Year}/{result.Code}",
                result);
        })
        .WithName("CopiarHojaCorte")
        .RequireAuthorization("HOJASCORTE.CREATE");

        // Anulación: mismo permiso UPDATE que el resto de transacciones.
        group.MapDelete("/{year}/{code}", async (string year, string code, ISender sender) =>
            Results.Ok(await sender.Send(new AnularHojaCorteCommand(year, code))))
        .WithName("AnularHojaCorte")
        .RequireAuthorization("HOJASCORTE.UPDATE");

        // ---------------------------------------------------------------- líneas

        group.MapPost("/{year}/{code}/detalles", async (
            string year,
            string code,
            HojaCorteDetalleRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new CreateHojaCorteDetalleCommand(year, code, body))))
        .WithName("CreateHojaCorteDetalle")
        .RequireAuthorization("HOJASCORTE.UPDATE");

        group.MapPut("/{year}/{code}/detalles/{productoTipoCode}/{productoCode}/{itemNumber:int}", async (
            string year,
            string code,
            string productoTipoCode,
            string productoCode,
            int itemNumber,
            UpdateHojaCorteDetalleRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new UpdateHojaCorteDetalleCommand(
                year, code, productoTipoCode, productoCode, itemNumber, body))))
        .WithName("UpdateHojaCorteDetalle")
        .RequireAuthorization("HOJASCORTE.UPDATE");

        group.MapDelete("/{year}/{code}/detalles/{productoTipoCode}/{productoCode}/{itemNumber:int}", async (
            string year,
            string code,
            string productoTipoCode,
            string productoCode,
            int itemNumber,
            ISender sender) =>
            Results.Ok(await sender.Send(new DeleteHojaCorteDetalleCommand(
                year, code, productoTipoCode, productoCode, itemNumber))))
        .WithName("DeleteHojaCorteDetalle")
        .RequireAuthorization("HOJASCORTE.UPDATE");
    }
}
