using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.CreateTipoPedido;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.ExportTiposPedido;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.GetTipoPedidoByCode;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.GetTiposPedidoPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.ToggleTipoPedidoStatus;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.UpdateTipoPedido;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class TiposPedidoEndpoints
    {
        public static void MapTiposPedidoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/tipos-pedido")
                .WithTags("Tipos de Pedido (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoPedidoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/tipos-pedido/{result.Code}", result);
            })
            .WithName("CreateTipoPedido")
            .RequireAuthorization("TIPOSPEDIDO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposPedidoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposPedidoPagedQueryString")
            .RequireAuthorization("TIPOSPEDIDO.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposPedidoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposPedidoPagedBody")
            .RequireAuthorization("TIPOSPEDIDO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposPedidoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposPedidoExcel")
            .RequireAuthorization("TIPOSPEDIDO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposPedidoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposPedidoPdf")
            .RequireAuthorization("TIPOSPEDIDO.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoPedidoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoPedidoByCode")
            .RequireAuthorization("TIPOSPEDIDO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoPedidoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoPedido")
            .RequireAuthorization("TIPOSPEDIDO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoPedidoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoPedidoStatus")
            .RequireAuthorization("TIPOSPEDIDO.UPDATE");
        }
    }
}