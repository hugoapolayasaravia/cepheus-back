using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.CreateCategoriaTrabajador;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.GetCategoriaTrabajadorByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.GetCategoriasTrabajadorPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.ToggleCategoriaTrabajadorStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.UpdateCategoriaTrabajador;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.ExportCategoriasTrabajador;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class CategoriasTrabajadorEndpoints
    {
        public static void MapCategoriasTrabajadorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/categorias-trabajador")
                .WithTags("CategoriasTrabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCategoriaTrabajadorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/categorias-trabajador/{result.Code}", result);
            })
            .WithName("CreateCategoriaTrabajador")
            .RequireAuthorization("CATEGORIASTRABAJADOR.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCategoriasTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCategoriasTrabajadorPagedQueryString")
            .RequireAuthorization("CATEGORIASTRABAJADOR.VIEW");

            group.MapPost("/paged/body", async (
                GetCategoriasTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCategoriasTrabajadorPagedBody")
            .RequireAuthorization("CATEGORIASTRABAJADOR.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportCategoriasTrabajadorQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportCategoriasTrabajadorExcel")
            .RequireAuthorization("CATEGORIASTRABAJADOR.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportCategoriasTrabajadorQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportCategoriasTrabajadorPdf")
            .RequireAuthorization("CATEGORIASTRABAJADOR.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCategoriaTrabajadorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCategoriaTrabajadorByCode")
            .RequireAuthorization("CATEGORIASTRABAJADOR.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCategoriaTrabajadorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCategoriaTrabajador")
            .RequireAuthorization("CATEGORIASTRABAJADOR.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCategoriaTrabajadorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCategoriaTrabajadorStatus")
            .RequireAuthorization("CATEGORIASTRABAJADOR.UPDATE");
        }
    }
}