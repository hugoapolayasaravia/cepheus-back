using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.CreateUnidadNegocio;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.ExportUnidadesNegocio;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.GetUnidadNegocioByCode;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.GetUnidadesNegocioPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.ToggleUnidadNegocioStatus;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.UpdateUnidadNegocio;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class UnidadesNegocioEndpoints
    {
        public static void MapUnidadesNegocioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/unidades-negocio")
                .WithTags("Unidades de Negocio (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateUnidadNegocioCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/unidades-negocio/{result.Code}", result);
            })
            .WithName("CreateUnidadNegocio")
            .RequireAuthorization("UNIDADESNEGOCIO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetUnidadesNegocioPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesNegocioPagedQueryString")
            .RequireAuthorization("UNIDADESNEGOCIO.VIEW");

            group.MapPost("/paged/body", async (
                GetUnidadesNegocioPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetUnidadesNegocioPagedBody")
            .RequireAuthorization("UNIDADESNEGOCIO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportUnidadesNegocioQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportUnidadesNegocioExcel")
            .RequireAuthorization("UNIDADESNEGOCIO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportUnidadesNegocioQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportUnidadesNegocioPdf")
            .RequireAuthorization("UNIDADESNEGOCIO.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetUnidadNegocioByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetUnidadNegocioByCode")
            .RequireAuthorization("UNIDADESNEGOCIO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateUnidadNegocioCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateUnidadNegocio")
            .RequireAuthorization("UNIDADESNEGOCIO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleUnidadNegocioStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleUnidadNegocioStatus")
            .RequireAuthorization("UNIDADESNEGOCIO.UPDATE");
        }
    }
}