using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.CreatePlanArticulo;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.ExportPlanesArticulo;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.GetPlanArticuloByCode;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.GetPlanesArticuloPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.TogglePlanArticuloStatus;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.UpdatePlanArticulo;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Catalogos
{
    public static class PlanesArticuloEndpoints
    {
        public static void MapPlanesArticuloEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/planes-articulo")
                .WithTags("Planes de Artículo (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePlanArticuloCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/planes-articulo/{result.Code}", result);
            })
            .WithName("CreatePlanArticulo")
            .RequireAuthorization("PLANESARTICULO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetPlanesArticuloPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPlanesArticuloPagedQueryString")
            .RequireAuthorization("PLANESARTICULO.VIEW");

            group.MapPost("/paged/body", async (
                GetPlanesArticuloPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPlanesArticuloPagedBody")
            .RequireAuthorization("PLANESARTICULO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportPlanesArticuloQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPlanesArticuloExcel")
            .RequireAuthorization("PLANESARTICULO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportPlanesArticuloQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPlanesArticuloPdf")
            .RequireAuthorization("PLANESARTICULO.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetPlanArticuloByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetPlanArticuloByCode")
            .RequireAuthorization("PLANESARTICULO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdatePlanArticuloCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePlanArticulo")
            .RequireAuthorization("PLANESARTICULO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new TogglePlanArticuloStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("TogglePlanArticuloStatus")
            .RequireAuthorization("PLANESARTICULO.UPDATE");
        }
    }
}