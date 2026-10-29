using Cepheus.Application.Features.Comunes.TiposDocumento.CreateTipoDocumento;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.TiposDocumento.ExportTiposDocumento;
using Cepheus.Application.Features.Comunes.TiposDocumento.GetTipoDocumentoById;
using Cepheus.Application.Features.Comunes.TiposDocumento.GetTiposDocumentoPaginated;
using Cepheus.Application.Features.Comunes.TiposDocumento.ToggleTipoDocumentoStatus;
using Cepheus.Application.Features.Comunes.TiposDocumento.UpdateTipoDocumento;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class TiposDocumentoEndpoints
    {
        public static void MapTiposDocumentoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/tipos-documento")
                .WithTags("TiposDocumento")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoDocumentoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comunes/tipos-documento/{result.Code}", result);
            })
            .WithName("CreateTipoDocumento")
            .RequireAuthorization("TIPOSDOCUMENTO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposDocumentoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposDocumentoPagedQueryString")
            .RequireAuthorization("TIPOSDOCUMENTO.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposDocumentoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposDocumentoPagedBody")
            .RequireAuthorization("TIPOSDOCUMENTO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposDocumentoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposDocumentoExcel")
            .RequireAuthorization("TIPOSDOCUMENTO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposDocumentoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposDocumentoPdf")
            .RequireAuthorization("TIPOSDOCUMENTO.EXPORT_PDF");

            group.MapGet("/{code}", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoDocumentoByIdQuery(code));
                return Results.Ok(result);
            })
            .WithName("GetTipoDocumentoById")
            .RequireAuthorization("TIPOSDOCUMENTO.VIEW");

            group.MapPut("/{code}", async (string code, UpdateTipoDocumentoCommand command, ISender sender) =>
            {
                if (!string.Equals(
                    code,
                    command.Code,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(
                        "El código de la ruta no coincide con el código del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoDocumento")
            .RequireAuthorization("TIPOSDOCUMENTO.UPDATE");

            group.MapPatch("/{code}/toggle-status", async (string code, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoDocumentoStatusCommand(code));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoDocumentoStatus")
            .RequireAuthorization("TIPOSDOCUMENTO.UPDATE");
        }
    }
}
