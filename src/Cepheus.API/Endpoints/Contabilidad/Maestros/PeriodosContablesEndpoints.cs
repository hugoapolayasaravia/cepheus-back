using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ClosePeriodoContable;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.CreatePeriodoContable;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodoContableByCode;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodosContablesPaginated;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ReopenPeriodoContable;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ExportPeriodosContables;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Maestros
{
    public static class PeriodosContablesEndpoints
    {
        public static void MapPeriodosContablesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/maestros/periodos-contables")
                .WithTags("Períodos contables")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePeriodoContableCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/maestros/periodos-contables/{result.Code}", result);
            })
            .WithName("CreatePeriodoContable")
            .RequireAuthorization("PERIODOSCONTABLES.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetPeriodosContablesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPeriodosContablesPagedQueryString")
            .RequireAuthorization("PERIODOSCONTABLES.VIEW");

            group.MapPost("/paged/body", async (GetPeriodosContablesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetPeriodosContablesPagedBody")
            .RequireAuthorization("PERIODOSCONTABLES.VIEW");

            // GET export/excel  (mismos filtros y orden que /paged, sin paginar)
            group.MapGet("/export/excel", async (
                [AsParameters] ExportPeriodosContablesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Excel;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPeriodosContablesExcel")
            .RequireAuthorization("PERIODOSCONTABLES.EXPORT_EXCEL");

            // GET export/pdf
            group.MapGet("/export/pdf", async (
                [AsParameters] ExportPeriodosContablesQuery query,
                ISender sender) =>
            {
                query.Format = ExportFormat.Pdf;
                var file = await sender.Send(query);
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("ExportPeriodosContablesPdf")
            .RequireAuthorization("PERIODOSCONTABLES.EXPORT_PDF");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetPeriodoContableByCodeQuery(codigo))))
            .WithName("GetPeriodoContableByCode")
            .RequireAuthorization("PERIODOSCONTABLES.VIEW");

            group.MapPatch("/{codigo}/close", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new ClosePeriodoContableCommand(codigo))))
            .WithName("ClosePeriodoContable")
            .RequireAuthorization("PERIODOSCONTABLES.UPDATE");

            group.MapPatch("/{codigo}/reopen", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new ReopenPeriodoContableCommand(codigo))))
            .WithName("ReopenPeriodoContable")
            .RequireAuthorization("PERIODOSCONTABLES.UPDATE");
        }
    }
}
