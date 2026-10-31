using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.CreateTipoAnexo;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.GetTipoAnexoById;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.GetTiposAnexoPaginated;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.ToggleTipoAnexoStatus;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.UpdateTipoAnexo;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.ExportTiposAnexo;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Catalogos
{
    public static class TiposAnexoEndpoints
    {
        public static void MapTiposAnexoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/catalogos/tipos-anexo")
                .WithTags("Tipos de anexo contable")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoAnexoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/catalogos/tipos-anexo/{result.Code}", result);
            })
            .WithName("CreateTipoAnexo")
            .RequireAuthorization("TIPOSANEXO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposAnexoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposAnexoPagedQueryString")
            .RequireAuthorization("TIPOSANEXO.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposAnexoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposAnexoPagedBody")
            .RequireAuthorization("TIPOSANEXO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposAnexoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposAnexoExcel")
            .RequireAuthorization("TIPOSANEXO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposAnexoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposAnexoPdf")
            .RequireAuthorization("TIPOSANEXO.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoAnexoByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoAnexoById")
            .RequireAuthorization("TIPOSANEXO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoAnexoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoAnexo")
            .RequireAuthorization("TIPOSANEXO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoAnexoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoAnexoStatus")
            .RequireAuthorization("TIPOSANEXO.UPDATE");
        }
    }
}
