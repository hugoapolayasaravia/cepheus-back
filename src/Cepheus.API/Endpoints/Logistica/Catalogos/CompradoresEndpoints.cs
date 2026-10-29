using Cepheus.Application.Features.Logistica.Catalogos.Compradores.CreateComprador;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.ExportCompradores;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.GetCompradorByCode;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.GetCompradoresPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.ToggleCompradorStatus;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.UpdateComprador;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class CompradoresEndpoints
    {
        public static void MapCompradoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/compradores")
                .WithTags("Compradores (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCompradorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/compradores/{result.Code}", result);
            })
            .WithName("CreateComprador")
            .RequireAuthorization("COMPRADORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCompradoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCompradoresPagedQueryString")
            .RequireAuthorization("COMPRADORES.VIEW");

            group.MapPost("/paged/body", async (
                GetCompradoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCompradoresPagedBody")
            .RequireAuthorization("COMPRADORES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportCompradoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportCompradoresExcel")
            .RequireAuthorization("COMPRADORES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportCompradoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportCompradoresPdf")
            .RequireAuthorization("COMPRADORES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCompradorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCompradorByCode")
            .RequireAuthorization("COMPRADORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCompradorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateComprador")
            .RequireAuthorization("COMPRADORES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCompradorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCompradorStatus")
            .RequireAuthorization("COMPRADORES.UPDATE");
        }
    }
}