using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.CreateTipoProducto;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.GetTipoProductoById;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.GetTiposProductoPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.ToggleTipoProductoStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.UpdateTipoProducto;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class TiposProductoEndpoints
    {
        public static void MapTiposProductoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/tipos-producto")
                .WithTags("Tipos de producto")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoProductoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/tipos-producto/{result.Code}", result);
            })
            .WithName("CreateTipoProducto")
            .RequireAuthorization("TIPOSPRODUCTO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposProductoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposProductoPagedQueryString")
            .RequireAuthorization("TIPOSPRODUCTO.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposProductoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposProductoPagedBody")
            .RequireAuthorization("TIPOSPRODUCTO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoProductoByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoProductoById")
            .RequireAuthorization("TIPOSPRODUCTO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoProductoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoProducto")
            .RequireAuthorization("TIPOSPRODUCTO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoProductoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoProductoStatus")
            .RequireAuthorization("TIPOSPRODUCTO.UPDATE");
        }
    }
}
