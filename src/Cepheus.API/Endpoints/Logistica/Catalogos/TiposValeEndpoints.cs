using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.CreateTipoVale;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.ExportTiposVale;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.GetTipoValeByCode;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.GetTiposValePaginated;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.ToggleTipoValeStatus;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.UpdateTipoVale;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class TiposValeEndpoints
    {
        public static void MapTiposValeEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/tipos-vale")
                .WithTags("Tipos de Vale (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoValeCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/tipos-vale/{result.Code}", result);
            })
            .WithName("CreateTipoVale")
            .RequireAuthorization("TIPOSVALE.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposValePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposValePagedQueryString")
            .RequireAuthorization("TIPOSVALE.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposValePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposValePagedBody")
            .RequireAuthorization("TIPOSVALE.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposValeQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposValeExcel")
            .RequireAuthorization("TIPOSVALE.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposValeQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposValePdf")
            .RequireAuthorization("TIPOSVALE.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoValeByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoValeByCode")
            .RequireAuthorization("TIPOSVALE.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoValeCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoVale")
            .RequireAuthorization("TIPOSVALE.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoValeStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoValeStatus")
            .RequireAuthorization("TIPOSVALE.UPDATE");
        }
    }
}