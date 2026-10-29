using Cepheus.Application.Features.Comunes.PeriodosIgv.CreatePeriodoIgv;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.PeriodosIgv.ExportPeriodosIgv;
using Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodoIgvByDate;
using Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodoIgvById;
using Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodosIgvPaginated;
using Cepheus.Application.Features.Comunes.PeriodosIgv.UpdatePeriodoIgv;
using MediatR;

namespace Cepheus.API.Endpoints.Comunes
{
    public static class PeriodosIgvEndpoints
    {
        public static void MapPeriodosIgvEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/comunes/periodos-igv")
                .WithTags("PeriodosIgv")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePeriodoIgvCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/comunes/periodos-igv/{result.Id}", result);
            })
            .WithName("CreatePeriodoIgv")
            .RequireAuthorization("PERIODOSIGV.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetPeriodosIgvPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPeriodosIgvPagedQueryString")
            .RequireAuthorization("PERIODOSIGV.VIEW");

            group.MapPost("/paged/body", async (GetPeriodosIgvPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPeriodosIgvPagedBody")
            .RequireAuthorization("PERIODOSIGV.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportPeriodosIgvQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPeriodosIgvExcel")
            .RequireAuthorization("PERIODOSIGV.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportPeriodosIgvQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPeriodosIgvPdf")
            .RequireAuthorization("PERIODOSIGV.EXPORT_PDF");

            group.MapGet("/{id:int}", async (int id, ISender sender) =>
                Results.Ok(await sender.Send(new GetPeriodoIgvByIdQuery(id))))
            .WithName("GetPeriodoIgvById")
            .RequireAuthorization("PERIODOSIGV.VIEW");

            group.MapGet("/by-date/{date}", async (DateOnly date, ISender sender) =>
                Results.Ok(await sender.Send(new GetPeriodoIgvByDateQuery(date))))
            .WithName("GetPeriodoIgvByDate")
            .RequireAuthorization("PERIODOSIGV.VIEW");

            group.MapPut("/{id:int}", async (int id, UpdatePeriodoIgvCommand command, ISender sender) =>
            {
                if (id != command.Id)
                    return Results.BadRequest("El Id de la ruta no coincide con el del cuerpo.");

                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdatePeriodoIgv")
            .RequireAuthorization("PERIODOSIGV.UPDATE");
        }
    }
}
