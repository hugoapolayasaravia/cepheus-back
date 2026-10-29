using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.CreateTecnico;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.GetTecnicoByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.GetTecnicosPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.ToggleTecnicoStatus;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class TecnicosEndpoints
    {
        public static void MapTecnicosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/tecnicos")
                .WithTags("Técnicos (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTecnicoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/maestros/tecnicos/{result.TrabajadorCode}", result);
            })
            .WithName("CreateTecnico")
            .RequireAuthorization("TECNICOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTecnicosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTecnicosPagedQueryString")
            .RequireAuthorization("TECNICOS.VIEW");

            group.MapPost("/paged/body", async (
                GetTecnicosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTecnicosPagedBody")
            .RequireAuthorization("TECNICOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTecnicoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTecnicoByCode")
            .RequireAuthorization("TECNICOS.VIEW");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTecnicoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTecnicoStatus")
            .RequireAuthorization("TECNICOS.UPDATE");
        }
    }
}
