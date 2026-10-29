using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.CreateCategoriaProducto;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.GetCategoriaProductoByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.GetCategoriasProductoPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.ToggleCategoriaProductoStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.UpdateCategoriaProducto;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class CategoriasProductoEndpoints
    {
        public static void MapCategoriasProductoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/categorias-producto")
                .WithTags("Categorías de producto")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCategoriaProductoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/categorias-producto/{result.Code}", result);
            })
            .WithName("CreateCategoriaProducto")
            .RequireAuthorization("CATEGORIASPRODUCTO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCategoriasProductoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCategoriasProductoPagedQueryString")
            .RequireAuthorization("CATEGORIASPRODUCTO.VIEW");

            group.MapPost("/paged/body", async (
                GetCategoriasProductoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCategoriasProductoPagedBody")
            .RequireAuthorization("CATEGORIASPRODUCTO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCategoriaProductoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCategoriaProductoByCode")
            .RequireAuthorization("CATEGORIASPRODUCTO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCategoriaProductoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCategoriaProducto")
            .RequireAuthorization("CATEGORIASPRODUCTO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCategoriaProductoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCategoriaProductoStatus")
            .RequireAuthorization("CATEGORIASPRODUCTO.UPDATE");
        }
    }
}
