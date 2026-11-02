using Cepheus.Application.Features.Logistica.Maestros.Transportistas.CreateTransportista;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.GetTransportistaByCode;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.GetTransportistasPaginated;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.ToggleTransportistaStatus;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.UpdateTransportista;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.ExportTransportistas;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class TransportistasEndpoints
    {
        public static void MapTransportistasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/transportistas")
                .WithTags("Transportistas (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTransportistaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/transportistas/{result.Code}", result);
            })
            .WithName("CreateTransportista")
            .RequireAuthorization("TRANSPORTISTAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTransportistasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTransportistasPagedQueryString")
            .RequireAuthorization("TRANSPORTISTAS.VIEW");

            group.MapPost("/paged/body", async (
                GetTransportistasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTransportistasPagedBody")
            .RequireAuthorization("TRANSPORTISTAS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTransportistasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTransportistasExcel")
            .RequireAuthorization("TRANSPORTISTAS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTransportistasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTransportistasPdf")
            .RequireAuthorization("TRANSPORTISTAS.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTransportistaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTransportistaByCode")
            .RequireAuthorization("TRANSPORTISTAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTransportistaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTransportista")
            .RequireAuthorization("TRANSPORTISTAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTransportistaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTransportistaStatus")
            .RequireAuthorization("TRANSPORTISTAS.UPDATE");
        }
    }
}
