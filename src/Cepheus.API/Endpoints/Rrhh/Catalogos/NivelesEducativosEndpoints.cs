using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.CreateNivelEducativo;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.GetNivelEducativoByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.GetNivelesEducativosPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.ToggleNivelEducativoStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.UpdateNivelEducativo;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.ExportNivelesEducativos;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class NivelesEducativosEndpoints
    {
        public static void MapNivelesEducativosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/niveles-educativos")
                .WithTags("NivelesEducativos (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNivelEducativoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/niveles-educativos/{result.Code}", result);
            })
            .WithName("CreateNivelEducativo")
            .RequireAuthorization("NIVELESEDUCATIVOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNivelesEducativosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNivelesEducativosPagedQueryString")
            .RequireAuthorization("NIVELESEDUCATIVOS.VIEW");

            group.MapPost("/paged/body", async (
                GetNivelesEducativosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNivelesEducativosPagedBody")
            .RequireAuthorization("NIVELESEDUCATIVOS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportNivelesEducativosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNivelesEducativosExcel")
            .RequireAuthorization("NIVELESEDUCATIVOS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportNivelesEducativosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNivelesEducativosPdf")
            .RequireAuthorization("NIVELESEDUCATIVOS.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetNivelEducativoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetNivelEducativoByCode")
            .RequireAuthorization("NIVELESEDUCATIVOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateNivelEducativoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNivelEducativo")
            .RequireAuthorization("NIVELESEDUCATIVOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleNivelEducativoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleNivelEducativoStatus")
            .RequireAuthorization("NIVELESEDUCATIVOS.UPDATE");
        }
    }
}