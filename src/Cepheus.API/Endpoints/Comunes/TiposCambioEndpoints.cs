using Cepheus.Application.Features.Comunes.TiposCambio.CreateTipoCambio;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.TiposCambio.ExportTiposCambio;
using Cepheus.Application.Features.Comunes.TiposCambio.GetTipoCambioByDate;
using Cepheus.Application.Features.Comunes.TiposCambio.GetTipoCambioById;
using Cepheus.Application.Features.Comunes.TiposCambio.GetTiposCambioPaginated;
using Cepheus.Application.Features.Comunes.TiposCambio.UpdateTipoCambio;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class TiposCambioEndpoints
    {
        public static void MapTiposCambioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/tipos-cambio")
                .WithTags("TiposCambio")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoCambioCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comunes/tipos-cambio/{result.Id}", result);
            })
            .WithName("CreateTipoCambio")
            .RequireAuthorization("TIPOSCAMBIO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposCambioPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCambioPagedQueryString")
            .RequireAuthorization("TIPOSCAMBIO.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposCambioPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCambioPagedBody")
            .RequireAuthorization("TIPOSCAMBIO.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportTiposCambioQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposCambioExcel")
            .RequireAuthorization("TIPOSCAMBIO.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportTiposCambioQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportTiposCambioPdf")
            .RequireAuthorization("TIPOSCAMBIO.EXPORT_PDF");

            group.MapGet("/{id:int}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoCambioByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetTipoCambioById")
            .RequireAuthorization("TIPOSCAMBIO.VIEW");

            // GET /api/comunes/tipos-cambio/by-date/2026-10-18?monedaCode=USD
            group.MapGet("/by-date/{date}", async (DateOnly date, string monedaCode, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoCambioByDateQuery(monedaCode, date));
                return Results.Ok(result);
            })
            .WithName("GetTipoCambioByDate")
            .RequireAuthorization("TIPOSCAMBIO.VIEW");

            group.MapPut("/{id:int}", async (int id, UpdateTipoCambioCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El Id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoCambio")
            .RequireAuthorization("TIPOSCAMBIO.UPDATE");
        }
    }
}
