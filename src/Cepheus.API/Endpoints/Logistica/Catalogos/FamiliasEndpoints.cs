using Cepheus.Application.Features.Logistica.Catalogos.Familias.CreateFamilia;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.ExportFamilias;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.GetFamiliaByCode;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.GetFamiliasPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.ToggleFamiliaStatus;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class FamiliasEndpoints
    {
        public static void MapFamiliasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/familias")
                .WithTags("Familias (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateFamiliaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/familias/{result.Code}", result);
            })
            .WithName("CreateFamilia")
            .RequireAuthorization("FAMILIAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetFamiliasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFamiliasPagedQueryString")
            .RequireAuthorization("FAMILIAS.VIEW");

            group.MapPost("/paged/body", async (
                GetFamiliasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFamiliasPagedBody")
            .RequireAuthorization("FAMILIAS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportFamiliasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportFamiliasExcel")
            .RequireAuthorization("FAMILIAS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportFamiliasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportFamiliasPdf")
            .RequireAuthorization("FAMILIAS.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetFamiliaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFamiliaByCode")
            .RequireAuthorization("FAMILIAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateFamiliaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFamilia")
            .RequireAuthorization("FAMILIAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleFamiliaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFamiliaStatus")
            .RequireAuthorization("FAMILIAS.UPDATE");
        }
    }
}