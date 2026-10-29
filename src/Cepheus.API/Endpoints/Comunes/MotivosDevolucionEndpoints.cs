using Cepheus.Application.Features.Comunes.MotivosDevolucion.CreateMotivoDevolucion;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.ExportMotivosDevolucion;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.GetMotivoDevolucionById;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.GetMotivosDevolucionPaginated;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.ToggleMotivoDevolucionStatus;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.UpdateMotivoDevolucion;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class MotivosDevolucionEndpoints
    {
        public static void MapMotivosDevolucionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/motivos-devolucion")
                .WithTags("MotivosDevolucion")
                .RequireAuthorization();

            group.MapPost("/", async (CreateMotivoDevolucionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comunes/motivos-devolucion/{result.Code}", result);
            })
            .WithName("CreateMotivoDevolucion")
            .RequireAuthorization("MOTIVOSDEVOLUCION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetMotivosDevolucionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMotivosDevolucionPagedQueryString")
            .RequireAuthorization("MOTIVOSDEVOLUCION.VIEW");

            group.MapPost("/paged/body", async (
                GetMotivosDevolucionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMotivosDevolucionPagedBody")
            .RequireAuthorization("MOTIVOSDEVOLUCION.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportMotivosDevolucionQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMotivosDevolucionExcel")
            .RequireAuthorization("MOTIVOSDEVOLUCION.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportMotivosDevolucionQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMotivosDevolucionPdf")
            .RequireAuthorization("MOTIVOSDEVOLUCION.EXPORT_PDF");

            group.MapGet("/{code}", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GetMotivoDevolucionByIdQuery(code));
                return Results.Ok(result);
            })
            .WithName("GetMotivoDevolucionById")
            .RequireAuthorization("MOTIVOSDEVOLUCION.VIEW");

            group.MapPut("/{code}", async (string code, UpdateMotivoDevolucionCommand command, ISender sender) =>
            {
                if (!string.Equals(code, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateMotivoDevolucion")
            .RequireAuthorization("MOTIVOSDEVOLUCION.UPDATE");

            group.MapPatch("/{code}/toggle-status", async (string code, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleMotivoDevolucionStatusCommand(code));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleMotivoDevolucionStatus")
            .RequireAuthorization("MOTIVOSDEVOLUCION.UPDATE");
        }
    }
}
