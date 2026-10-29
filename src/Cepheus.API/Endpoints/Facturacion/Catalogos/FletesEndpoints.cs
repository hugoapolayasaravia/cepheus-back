using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.CreateFlete;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.GetFleteByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.GetFletesPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.ToggleFleteStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.UpdateFlete;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class FletesEndpoints
    {
        public static void MapFletesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/fletes")
                .WithTags("Fletes (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateFleteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/fletes/{result.Code}", result);
            })
            .WithName("CreateFlete")
            .RequireAuthorization("FLETES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetFletesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFletesPagedQueryString")
            .RequireAuthorization("FLETES.VIEW");

            group.MapPost("/paged/body", async (
                GetFletesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFletesPagedBody")
            .RequireAuthorization("FLETES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetFleteByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFleteByCode")
            .RequireAuthorization("FLETES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateFleteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFlete")
            .RequireAuthorization("FLETES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleFleteStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFleteStatus")
            .RequireAuthorization("FLETES.UPDATE");
        }
    }
}
