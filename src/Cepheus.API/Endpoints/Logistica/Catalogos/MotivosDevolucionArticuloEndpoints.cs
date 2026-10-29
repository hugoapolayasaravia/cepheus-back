
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.CreateMotivoDevolucionArticulo;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.GetMotivoDevolucionArticuloById;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.GetMotivosDevolucionArticuloPaginated;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.ToggleMotivoDevolucionArticuloStatus;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.UpdateMotivoDevolucionArticulo;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.ExportMotivosDevolucionArticulo;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class MotivosDevolucionarticuloEndpoints
    {
        public static void MapMotivosDevolucionArticuloEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/catalogos/motivos-devolucion")
                .WithTags("MotivosDevolucionArticulo")
                .RequireAuthorization();

            group.MapPost("/", async (CreateMotivoDevolucionArticuloCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/catalogos/motivos-devolucion/{result.Code}", result);
            })
            .WithName("CreateMotivoDevolucionArticulo")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetMotivosDevolucionArticuloPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMotivosDevolucionArticuloPagedQueryString")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.VIEW");

            group.MapPost("/paged/body", async (
                GetMotivosDevolucionArticuloPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMotivosDevolucionArticuloPagedBody")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportMotivosDevolucionArticuloQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMotivosDevolucionArticuloExcel")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportMotivosDevolucionArticuloQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMotivosDevolucionArticuloPdf")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.EXPORT_PDF");

            group.MapGet("/{code}", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GetMotivoDevolucionArticuloByIdQuery(code));
                return Results.Ok(result);
            })
            .WithName("GetMotivoDevolucionArticuloById")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.VIEW");

            group.MapPut("/{code}", async (string code, UpdateMotivoDevolucionArticuloCommand command, ISender sender) =>
            {
                if (!string.Equals(code, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateMotivoDevolucionArticulo")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.UPDATE");

            group.MapPatch("/{code}/toggle-status", async (string code, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleMotivoDevolucionArticuloStatusCommand(code));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleMotivoDevolucionArticuloStatus")
            .RequireAuthorization("MOTIVOSDEVOLUCIONART.UPDATE");
        }
    }
}
