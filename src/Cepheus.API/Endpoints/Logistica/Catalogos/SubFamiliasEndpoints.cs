using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.CreateSubFamilia;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.ExportSubFamilias;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.GetSubFamiliaByCode;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.GetSubFamiliasPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.ToggleSubFamiliaStatus;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.UpdateSubFamilia;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class SubFamiliasEndpoints
    {
        public static void MapSubFamiliasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/subfamilias")
                .WithTags("SubFamilias (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSubFamiliaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/subfamilias/{result.Code}", result);
            })
            .WithName("CreateSubFamilia")
            .RequireAuthorization("SUBFAMILIAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSubFamiliasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubFamiliasPagedQueryString")
            .RequireAuthorization("SUBFAMILIAS.VIEW");

            group.MapPost("/paged/body", async (
                GetSubFamiliasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubFamiliasPagedBody")
            .RequireAuthorization("SUBFAMILIAS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportSubFamiliasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportSubFamiliasExcel")
            .RequireAuthorization("SUBFAMILIAS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportSubFamiliasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportSubFamiliasPdf")
            .RequireAuthorization("SUBFAMILIAS.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSubFamiliaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSubFamiliaByCode")
            .RequireAuthorization("SUBFAMILIAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSubFamiliaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSubFamilia")
            .RequireAuthorization("SUBFAMILIAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSubFamiliaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSubFamiliaStatus")
            .RequireAuthorization("SUBFAMILIAS.UPDATE");
        }
    }
}