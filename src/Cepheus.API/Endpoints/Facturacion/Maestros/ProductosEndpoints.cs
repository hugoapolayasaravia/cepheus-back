using Cepheus.Application.Features.Facturacion.Maestros.Productos.CreateProducto;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.GetProductoByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.GetProductosPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.ToggleProductoStatus;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.UpdateProducto;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class ProductosEndpoints
    {
        public static void MapProductosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/productos")
                .WithTags("Productos")
                .RequireAuthorization();

            group.MapPost("/", async (CreateProductoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/productos/{result.TipoProductoCode}/{result.Code}", result);
            })
            .WithName("CreateProducto")
            .RequireAuthorization("PRODUCTOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetProductosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetProductosPagedQueryString")
            .RequireAuthorization("PRODUCTOS.VIEW");

            group.MapPost("/paged/body", async (
                GetProductosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetProductosPagedBody")
            .RequireAuthorization("PRODUCTOS.VIEW");

            group.MapGet("/{tipoProducto}/{codigo}", async (
                string tipoProducto, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetProductoByCodeQuery(tipoProducto, codigo));
                return Results.Ok(result);
            })
            .WithName("GetProductoByCode")
            .RequireAuthorization("PRODUCTOS.VIEW");

            group.MapPut("/{tipoProducto}/{codigo}", async (
                string tipoProducto, string codigo, UpdateProductoCommand command, ISender sender) =>
            {
                if (!string.Equals(tipoProducto, command.TipoProductoCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateProducto")
            .RequireAuthorization("PRODUCTOS.UPDATE");

            group.MapPatch("/{tipoProducto}/{codigo}/toggle-status", async (
                string tipoProducto, string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleProductoStatusCommand(tipoProducto, codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleProductoStatus")
            .RequireAuthorization("PRODUCTOS.UPDATE");
        }
    }
}
