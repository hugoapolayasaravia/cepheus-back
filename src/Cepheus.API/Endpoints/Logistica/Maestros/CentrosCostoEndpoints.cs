using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.CreateCentroCosto;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.GetCentroCostoByCode;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.GetCentrosCostoPaginated;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.ToggleCentroCostoStatus;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.UpdateCentroCosto;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class CentrosCostoEndpoints
    {
        public static void MapCentrosCostoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/centros-costo")
                .WithTags("Centros de Costo (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCentroCostoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/centros-costo/{result.Code}", result);
            })
            .WithName("CreateCentroCosto")
            .RequireAuthorization("CENTROSCOSTO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCentrosCostoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCentrosCostoPagedQueryString")
            .RequireAuthorization("CENTROSCOSTO.VIEW");

            group.MapPost("/paged/body", async (
                GetCentrosCostoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCentrosCostoPagedBody")
            .RequireAuthorization("CENTROSCOSTO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCentroCostoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCentroCostoByCode")
            .RequireAuthorization("CENTROSCOSTO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCentroCostoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCentroCosto")
            .RequireAuthorization("CENTROSCOSTO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCentroCostoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCentroCostoStatus")
            .RequireAuthorization("CENTROSCOSTO.UPDATE");
        }
    }
}
