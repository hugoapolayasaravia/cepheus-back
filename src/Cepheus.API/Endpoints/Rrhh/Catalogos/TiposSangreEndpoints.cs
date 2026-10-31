using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.CreateTipoSangre;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.GetTipoSangreByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.GetTiposSangrePaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.ToggleTipoSangreStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.UpdateTipoSangre;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.ExportTiposSangre;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposSangreEndpoints
    {
        public static void MapTiposSangreEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-sangre")
                .WithTags("TiposSangre (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoSangreCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-sangre/{result.Code}", result);
            })
            .WithName("CreateTipoSangre")
            .RequireAuthorization("TIPOSSANGRE.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposSangrePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposSangrePagedQueryString")
            .RequireAuthorization("TIPOSSANGRE.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposSangrePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposSangrePagedBody")
            .RequireAuthorization("TIPOSSANGRE.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposSangreQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposSangreExcel")
            .RequireAuthorization("TIPOSSANGRE.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposSangreQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposSangrePdf")
            .RequireAuthorization("TIPOSSANGRE.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoSangreByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoSangreByCode")
            .RequireAuthorization("TIPOSSANGRE.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoSangreCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoSangre")
            .RequireAuthorization("TIPOSSANGRE.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoSangreStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoSangreStatus")
            .RequireAuthorization("TIPOSSANGRE.UPDATE");
        }
    }
}