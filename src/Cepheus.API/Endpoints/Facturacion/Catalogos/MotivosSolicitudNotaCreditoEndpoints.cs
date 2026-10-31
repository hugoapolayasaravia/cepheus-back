using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.CreateMotivoSolicitudNotaCredito;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.GetMotivoSolicitudNotaCreditoByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.GetMotivosSolicitudNotaCreditoPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.ToggleMotivoSolicitudNotaCreditoStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.UpdateMotivoSolicitudNotaCredito;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.ExportMotivosSolicitudNotaCredito;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class MotivosSolicitudNotaCreditoEndpoints
    {
        public static void MapMotivosSolicitudNotaCreditoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/motivos-solicitud-nota-credito")
                .WithTags("Motivos de Solicitud de Nota de Crédito (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateMotivoSolicitudNotaCreditoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/motivos-solicitud-nota-credito/{result.Code}", result);
            })
            .WithName("CreateMotivoSolicitudNotaCredito")
            .RequireAuthorization("MOTIVOSSNC.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetMotivosSolicitudNotaCreditoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMotivosSolicitudNotaCreditoPagedQueryString")
            .RequireAuthorization("MOTIVOSSNC.VIEW");

            group.MapPost("/paged/body", async (
                GetMotivosSolicitudNotaCreditoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMotivosSolicitudNotaCreditoPagedBody")
            .RequireAuthorization("MOTIVOSSNC.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportMotivosSolicitudNotaCreditoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMotivosSolicitudNotaCreditoExcel")
            .RequireAuthorization("MOTIVOSSNC.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportMotivosSolicitudNotaCreditoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMotivosSolicitudNotaCreditoPdf")
            .RequireAuthorization("MOTIVOSSNC.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetMotivoSolicitudNotaCreditoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetMotivoSolicitudNotaCreditoByCode")
            .RequireAuthorization("MOTIVOSSNC.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateMotivoSolicitudNotaCreditoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateMotivoSolicitudNotaCredito")
            .RequireAuthorization("MOTIVOSSNC.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleMotivoSolicitudNotaCreditoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleMotivoSolicitudNotaCreditoStatus")
            .RequireAuthorization("MOTIVOSSNC.UPDATE");
        }
    }
}
