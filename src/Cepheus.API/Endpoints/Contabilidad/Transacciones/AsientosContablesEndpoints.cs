using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.AnularAsientoContable;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.GetAsientoContableByCode;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.GetAsientosContablesPaginated;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class AsientosContablesEndpoints
    {
        public static void MapAsientosContablesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/transacciones/asientos-contables")
                .WithTags("Asientos contables")
                .RequireAuthorization();

            group.MapGet("/paged", async ([AsParameters] GetAsientosContablesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetAsientosContablesPagedQueryString")
            .RequireAuthorization("ASIENTOSCONTABLES.VIEW");

            group.MapPost("/paged/body", async (GetAsientosContablesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetAsientosContablesPagedBody")
            .RequireAuthorization("ASIENTOSCONTABLES.VIEW");

            // codigo = año(4) + mes(2) + tipo(2) + número(5), por ejemplo 2026100200001
            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetAsientoContableByCodeQuery(codigo))))
            .WithName("GetAsientoContableByCode")
            .RequireAuthorization("ASIENTOSCONTABLES.VIEW");

            group.MapPatch("/{codigo}/anular", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new AnularAsientoContableCommand(codigo))))
            .WithName("AnularAsientoContable")
            .RequireAuthorization("ASIENTOSCONTABLES.UPDATE");
        }
    }
}
