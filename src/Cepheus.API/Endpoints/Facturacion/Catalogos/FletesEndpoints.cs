using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.CreateFlete;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.GetFleteByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.GetFletesPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.ToggleFleteStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.UpdateFlete;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.ExportFletes;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class FletesEndpoints
    {
        public static void MapFletesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/fletes")
                .WithTags("Fletes (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateFleteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/fletes/{result.Code}", result);
            })
            .WithName("CreateFlete")
            .RequireAuthorization("FLETES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetFletesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFletesPagedQueryString")
            .RequireAuthorization("FLETES.VIEW");

            group.MapPost("/paged/body", async (
                GetFletesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFletesPagedBody")
            .RequireAuthorization("FLETES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportFletesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportFletesExcel")
            .RequireAuthorization("FLETES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportFletesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportFletesPdf")
            .RequireAuthorization("FLETES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetFleteByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFleteByCode")
            .RequireAuthorization("FLETES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateFleteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFlete")
            .RequireAuthorization("FLETES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleFleteStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFleteStatus")
            .RequireAuthorization("FLETES.UPDATE");
        }
    }
}
