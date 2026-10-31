using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.CreateNotaCotizacionPlantilla;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.GetNotaCotizacionPlantillaByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.GetNotasCotizacionPlantillaPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.ToggleNotaCotizacionPlantillaStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.UpdateNotaCotizacionPlantilla;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.ExportNotasCotizacionPlantilla;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class NotasCotizacionPlantillaEndpoints
    {
        public static void MapNotasCotizacionPlantillaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/notas-cotizacion-plantilla")
                .WithTags("Notas de Cotización - Plantilla (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNotaCotizacionPlantillaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/catalogos/notas-cotizacion-plantilla/{result.NegocioCode}/{result.Code}", result);
            })
            .WithName("CreateNotaCotizacionPlantilla")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNotasCotizacionPlantillaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNotasCotizacionPlantillaPagedQueryString")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.VIEW");

            group.MapPost("/paged/body", async (
                GetNotasCotizacionPlantillaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNotasCotizacionPlantillaPagedBody")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportNotasCotizacionPlantillaQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNotasCotizacionPlantillaExcel")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportNotasCotizacionPlantillaQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportNotasCotizacionPlantillaPdf")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.EXPORT_PDF");

            group.MapGet("/{negocioCodigo}/{codigo}", async (string negocioCodigo, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetNotaCotizacionPlantillaByCodeQuery(negocioCodigo, codigo));
                return Results.Ok(result);
            })
            .WithName("GetNotaCotizacionPlantillaByCode")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.VIEW");

            group.MapPut("/{negocioCodigo}/{codigo}", async (
                string negocioCodigo, string codigo, UpdateNotaCotizacionPlantillaCommand command, ISender sender) =>
            {
                if (negocioCodigo != command.NegocioCode || codigo != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNotaCotizacionPlantilla")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.UPDATE");

            group.MapPatch("/{negocioCodigo}/{codigo}/toggle-status", async (
                string negocioCodigo, string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleNotaCotizacionPlantillaStatusCommand(negocioCodigo, codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleNotaCotizacionPlantillaStatus")
            .RequireAuthorization("NOTASCOTIZAPLANTILLA.UPDATE");
        }
    }
}
