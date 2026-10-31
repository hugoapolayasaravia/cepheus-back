using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.CreateTipoContrato;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.GetTipoContratoByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.GetTiposContratoPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.ToggleTipoContratoStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.UpdateTipoContrato;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.ExportTiposContrato;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposContratoEndpoints
    {
        public static void MapTiposContratoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-contrato")
                .WithTags("TiposContrato (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoContratoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-contrato/{result.Code}", result);
            })
            .WithName("CreateTipoContrato")
            .RequireAuthorization("TIPOSCONTRATO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposContratoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposContratoPagedQueryString")
            .RequireAuthorization("TIPOSCONTRATO.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposContratoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposContratoPagedBody")
            .RequireAuthorization("TIPOSCONTRATO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposContratoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposContratoExcel")
            .RequireAuthorization("TIPOSCONTRATO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposContratoQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposContratoPdf")
            .RequireAuthorization("TIPOSCONTRATO.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoContratoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoContratoByCode")
            .RequireAuthorization("TIPOSCONTRATO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoContratoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoContrato")
            .RequireAuthorization("TIPOSCONTRATO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoContratoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoContratoStatus")
            .RequireAuthorization("TIPOSCONTRATO.UPDATE");
        }
    }
}