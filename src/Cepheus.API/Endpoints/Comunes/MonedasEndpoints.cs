using Cepheus.Application.Features.Comunes.Monedas.CreateMoneda;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Monedas.ExportMonedas;
using Cepheus.Application.Features.Comunes.Monedas.GetMonedaById;
using Cepheus.Application.Features.Comunes.Monedas.GetMonedasPaginated;
using Cepheus.Application.Features.Comunes.Monedas.ToggleMonedaStatus;
using Cepheus.Application.Features.Comunes.Monedas.UpdateMoneda;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class MonedasEndpoints
    {
        public static void MapMonedasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/monedas")
                .WithTags("Monedas")
                .RequireAuthorization();

            group.MapPost("/", async (CreateMonedaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comunes/monedas/{result.Code}", result);
            })
            .WithName("CreateMoneda")
            .RequireAuthorization("MONEDAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetMonedasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMonedasPagedQueryString")
            .RequireAuthorization("MONEDAS.VIEW");

            group.MapPost("/paged/body", async (
                GetMonedasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetMonedasPagedBody")
            .RequireAuthorization("MONEDAS.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportMonedasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMonedasExcel")
            .RequireAuthorization("MONEDAS.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportMonedasQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportMonedasPdf")
            .RequireAuthorization("MONEDAS.EXPORT_PDF");

            group.MapGet("/{code}", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GetMonedaByIdQuery(code));
                return Results.Ok(result);
            })
            .WithName("GetMonedaById")
            .RequireAuthorization("MONEDAS.VIEW");

            group.MapPut("/{code}", async (string code, UpdateMonedaCommand command, ISender sender) =>
            {
                if (!string.Equals(
                    code,
                    command.Code,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(
                        "El código de la ruta no coincide con el código del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateMoneda")
            .RequireAuthorization("MONEDAS.UPDATE");

            group.MapPatch("/{code}/toggle-status", async (string code, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleMonedaStatusCommand(code));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleMonedaStatus")
            .RequireAuthorization("MONEDAS.UPDATE");
        }
    }
}
