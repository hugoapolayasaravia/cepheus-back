using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.CreateStockProducto;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.GetStockProductoById;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.GetStockProductosPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.UpdateStockProducto;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.ExportStockProductos;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class StockProductosEndpoints
    {
        public static void MapStockProductosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/stock-productos")
                .WithTags("Stock de productos")
                .RequireAuthorization();

            group.MapPost("/", async (CreateStockProductoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/stock-productos/{result.Id}", result);
            })
            .WithName("CreateStockProducto")
            .RequireAuthorization("STOCKPRODUCTOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetStockProductosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetStockProductosPagedQueryString")
            .RequireAuthorization("STOCKPRODUCTOS.VIEW");

            group.MapPost("/paged/body", async (
                GetStockProductosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetStockProductosPagedBody")
            .RequireAuthorization("STOCKPRODUCTOS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportStockProductosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportStockProductosExcel")
            .RequireAuthorization("STOCKPRODUCTOS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportStockProductosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportStockProductosPdf")
            .RequireAuthorization("STOCKPRODUCTOS.EXPORT_PDF");

            group.MapGet("/{id:long}", async (long id, ISender sender) =>
            {
                var result = await sender.Send(new GetStockProductoByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetStockProductoById")
            .RequireAuthorization("STOCKPRODUCTOS.VIEW");

            group.MapPut("/{id:long}", async (long id, UpdateStockProductoCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El identificador de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateStockProducto")
            .RequireAuthorization("STOCKPRODUCTOS.UPDATE");
        }
    }
}
