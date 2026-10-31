using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.CreateCobrador;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.GetCobradorByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.GetCobradoresPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.ToggleCobradorStatus;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.UpdateCobrador;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.ExportCobradores;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class CobradoresEndpoints
    {
        public static void MapCobradoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/cobradores")
                .WithTags("Cobradores")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCobradorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/maestros/cobradores/{result.Code}", result);
            })
            .WithName("CreateCobrador")
            .RequireAuthorization("COBRADORES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCobradoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCobradoresPagedQueryString")
            .RequireAuthorization("COBRADORES.VIEW");

            group.MapPost("/paged/body", async (
                GetCobradoresPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCobradoresPagedBody")
            .RequireAuthorization("COBRADORES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportCobradoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportCobradoresExcel")
            .RequireAuthorization("COBRADORES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportCobradoresQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportCobradoresPdf")
            .RequireAuthorization("COBRADORES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCobradorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCobradorByCode")
            .RequireAuthorization("COBRADORES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCobradorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCobrador")
            .RequireAuthorization("COBRADORES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCobradorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCobradorStatus")
            .RequireAuthorization("COBRADORES.UPDATE");
        }
    }
}
