using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.CreateSubCentroCosto;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.GetSubCentroCostoByCode;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.GetSubCentrosCostoPaginated;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.ToggleSubCentroCostoStatus;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.UpdateSubCentroCosto;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class SubCentrosCostoEndpoints
    {
        public static void MapSubCentrosCostoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/subcentros-costo")
                .WithTags("SubCentros de Costo (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSubCentroCostoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/subcentros-costo/{result.Code}", result);
            })
            .WithName("CreateSubCentroCosto")
            .RequireAuthorization("SUBCENTROSCOSTO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSubCentrosCostoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubCentrosCostoPagedQueryString")
            .RequireAuthorization("SUBCENTROSCOSTO.VIEW");

            group.MapPost("/paged/body", async (
                GetSubCentrosCostoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSubCentrosCostoPagedBody")
            .RequireAuthorization("SUBCENTROSCOSTO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSubCentroCostoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSubCentroCostoByCode")
            .RequireAuthorization("SUBCENTROSCOSTO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSubCentroCostoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSubCentroCosto")
            .RequireAuthorization("SUBCENTROSCOSTO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSubCentroCostoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSubCentroCostoStatus")
            .RequireAuthorization("SUBCENTROSCOSTO.UPDATE");
        }
    }
}
