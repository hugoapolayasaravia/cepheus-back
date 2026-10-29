using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.CreateTipoCompra;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.ExportTiposCompra;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.GetTipoCompraByCode;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.GetTiposCompraPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.ToggleTipoCompraStatus;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.UpdateTipoCompra;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class TiposCompraEndpoints
    {
        public static void MapTiposCompraEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/tipos-compra")
                .WithTags("Tipos de Compra (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoCompraCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/tipos-compra/{result.Code}", result);
            })
            .WithName("CreateTipoCompra")
            .RequireAuthorization("TIPOSCOMPRA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposCompraPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCompraPagedQueryString")
            .RequireAuthorization("TIPOSCOMPRA.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposCompraPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCompraPagedBody")
            .RequireAuthorization("TIPOSCOMPRA.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposCompraQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposCompraExcel")
            .RequireAuthorization("TIPOSCOMPRA.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposCompraQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposCompraPdf")
            .RequireAuthorization("TIPOSCOMPRA.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoCompraByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoCompraByCode")
            .RequireAuthorization("TIPOSCOMPRA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoCompraCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoCompra")
            .RequireAuthorization("TIPOSCOMPRA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoCompraStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoCompraStatus")
            .RequireAuthorization("TIPOSCOMPRA.UPDATE");
        }
    }
}