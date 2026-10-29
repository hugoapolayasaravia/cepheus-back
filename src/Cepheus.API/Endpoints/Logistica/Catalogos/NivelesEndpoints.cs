using Cepheus.Application.Features.Logistica.Catalogos.Niveles.CreateNivel;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.ExportNiveles;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.GetNivelByCode;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.GetNivelesPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.ToggleNivelStatus;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.UpdateNivel;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class NivelesEndpoints
    {
        public static void MapNivelesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/niveles")
                .WithTags("Niveles (Logística - Aprobaciones)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNivelCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/niveles/{result.Code}", result);
            })
            .WithName("CreateNivel")
            .RequireAuthorization("NIVELES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNivelesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNivelesPagedQueryString")
            .RequireAuthorization("NIVELES.VIEW");

            group.MapPost("/paged/body", async (
                GetNivelesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNivelesPagedBody")
            .RequireAuthorization("NIVELES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportNivelesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNivelesExcel")
            .RequireAuthorization("NIVELES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportNivelesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNivelesPdf")
            .RequireAuthorization("NIVELES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetNivelByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetNivelByCode")
            .RequireAuthorization("NIVELES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateNivelCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNivel")
            .RequireAuthorization("NIVELES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleNivelStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleNivelStatus")
            .RequireAuthorization("NIVELES.UPDATE");
        }
    }
}
