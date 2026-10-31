using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.CreateTitulo;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.GetTituloByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.GetTitulosPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.ToggleTituloStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.UpdateTitulo;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.ExportTitulos;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TitulosEndpoints
    {
        public static void MapTitulosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/titulos")
                .WithTags("Titulos (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTituloCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/titulos/{result.Code}", result);
            })
            .WithName("CreateTitulo")
            .RequireAuthorization("TITULOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTitulosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTitulosPagedQueryString")
            .RequireAuthorization("TITULOS.VIEW");

            group.MapPost("/paged/body", async (
                GetTitulosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTitulosPagedBody")
            .RequireAuthorization("TITULOS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTitulosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTitulosExcel")
            .RequireAuthorization("TITULOS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTitulosQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTitulosPdf")
            .RequireAuthorization("TITULOS.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTituloByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTituloByCode")
            .RequireAuthorization("TITULOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTituloCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTitulo")
            .RequireAuthorization("TITULOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTituloStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTituloStatus")
            .RequireAuthorization("TITULOS.UPDATE");
        }
    }
}