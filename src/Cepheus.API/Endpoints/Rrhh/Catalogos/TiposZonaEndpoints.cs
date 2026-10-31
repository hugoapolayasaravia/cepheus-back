using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.CreateTipoZona;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.GetTipoZonaByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.GetTiposZonaPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.ToggleTipoZonaStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.UpdateTipoZona;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.ExportTiposZona;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposZonaEndpoints
    {
        public static void MapTiposZonaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-zona")
                .WithTags("TiposZona (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoZonaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-zona/{result.Code}", result);
            })
            .WithName("CreateTipoZona")
            .RequireAuthorization("TIPOSZONA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposZonaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposZonaPagedQueryString")
            .RequireAuthorization("TIPOSZONA.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposZonaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposZonaPagedBody")
            .RequireAuthorization("TIPOSZONA.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposZonaQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposZonaExcel")
            .RequireAuthorization("TIPOSZONA.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposZonaQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposZonaPdf")
            .RequireAuthorization("TIPOSZONA.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoZonaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoZonaByCode")
            .RequireAuthorization("TIPOSZONA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoZonaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoZona")
            .RequireAuthorization("TIPOSZONA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoZonaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoZonaStatus")
            .RequireAuthorization("TIPOSZONA.UPDATE");
        }
    }
}