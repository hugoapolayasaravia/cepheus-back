using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.CreateTipoOperacion;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.GetTipoOperacionById;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.GetTiposOperacionPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.ToggleTipoOperacionStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.UpdateTipoOperacion;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class TiposOperacionEndpoints
    {
        public static void MapTiposOperacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/tipos-operacion")
                .WithTags("Tipos de operación")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoOperacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/tipos-operacion/{result.Code}", result);
            })
            .WithName("CreateTipoOperacion")
            .RequireAuthorization("TIPOSOPERACION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposOperacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposOperacionPagedQueryString")
            .RequireAuthorization("TIPOSOPERACION.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposOperacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposOperacionPagedBody")
            .RequireAuthorization("TIPOSOPERACION.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoOperacionByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoOperacionById")
            .RequireAuthorization("TIPOSOPERACION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoOperacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoOperacion")
            .RequireAuthorization("TIPOSOPERACION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoOperacionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoOperacionStatus")
            .RequireAuthorization("TIPOSOPERACION.UPDATE");
        }
    }
}
