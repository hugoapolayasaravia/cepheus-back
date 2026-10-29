using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ClosePeriodoContable;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.CreatePeriodoContable;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodoContableByCode;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodosContablesPaginated;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ReopenPeriodoContable;
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
