using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.CreateTipoTransaccion;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.ExportTiposTransaccion;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.GetTipoTransaccionByCode;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.GetTiposTransaccionPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.ToggleTipoTransaccionStatus;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.UpdateTipoTransaccion;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class TiposTransaccionEndpoints
    {
        public static void MapTiposTransaccionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/tipos-transaccion")
                .WithTags("Tipos de Transacción (Logística - Aprobaciones)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoTransaccionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/tipos-transaccion/{result.Code}", result);
            })
            .WithName("CreateTipoTransaccion")
            .RequireAuthorization("TIPOSTRANSACCION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposTransaccionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposTransaccionPagedQueryString")
            .RequireAuthorization("TIPOSTRANSACCION.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposTransaccionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposTransaccionPagedBody")
            .RequireAuthorization("TIPOSTRANSACCION.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposTransaccionQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposTransaccionExcel")
            .RequireAuthorization("TIPOSTRANSACCION.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposTransaccionQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposTransaccionPdf")
            .RequireAuthorization("TIPOSTRANSACCION.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoTransaccionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoTransaccionByCode")
            .RequireAuthorization("TIPOSTRANSACCION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoTransaccionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoTransaccion")
            .RequireAuthorization("TIPOSTRANSACCION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoTransaccionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoTransaccionStatus")
            .RequireAuthorization("TIPOSTRANSACCION.UPDATE");
        }
    }
}
