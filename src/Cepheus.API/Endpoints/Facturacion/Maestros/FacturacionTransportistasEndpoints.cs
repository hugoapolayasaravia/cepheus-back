using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.CreateTransportista;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.GetTransportistaByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.GetTransportistasPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.ToggleTransportistaStatus;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.UpdateTransportista;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.ExportTransportistas;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class FacturacionTransportistasEndpoints
    {
        public static void MapFacturacionTransportistasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/transportistas")
                .WithTags("Transportistas (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTransportistaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/maestros/transportistas/{result.Code}", result);
            })
            .WithName("CreateFacturacionTransportista")
            .RequireAuthorization("FAC_TRANSPORTISTAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTransportistasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionTransportistasPagedQueryString")
            .RequireAuthorization("FAC_TRANSPORTISTAS.VIEW");

            group.MapPost("/paged/body", async (
                GetTransportistasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionTransportistasPagedBody")
            .RequireAuthorization("FAC_TRANSPORTISTAS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTransportistasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportFacturacionTransportistasExcel")
            .RequireAuthorization("FAC_TRANSPORTISTAS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTransportistasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportFacturacionTransportistasPdf")
            .RequireAuthorization("FAC_TRANSPORTISTAS.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTransportistaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFacturacionTransportistaByCode")
            .RequireAuthorization("FAC_TRANSPORTISTAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTransportistaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFacturacionTransportista")
            .RequireAuthorization("FAC_TRANSPORTISTAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTransportistaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFacturacionTransportistaStatus")
            .RequireAuthorization("FAC_TRANSPORTISTAS.UPDATE");
        }
    }
}
