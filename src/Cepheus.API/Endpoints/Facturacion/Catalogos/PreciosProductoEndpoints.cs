using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.CreatePrecioProducto;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.DeletePrecioProducto;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.GetPrecioProducto;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.GetPreciosProductoPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.UpdatePrecioProducto;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class PreciosProductoEndpoints
    {
        public static void MapPreciosProductoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/precios-producto")
                .WithTags("Precios de Producto (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreatePrecioProductoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/catalogos/precios-producto/{result.FleteCode}/{result.ProductoTipoCode}/{result.ProductoCode}/{result.CurrencyTypeCode}/{result.CurrencyCode}",
                    result);
            })
            .WithName("CreatePrecioProducto")
            .RequireAuthorization("PRECIOSPRODUCTO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetPreciosProductoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPreciosProductoPagedQueryString")
            .RequireAuthorization("PRECIOSPRODUCTO.VIEW");

            group.MapPost("/paged/body", async (
                GetPreciosProductoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPreciosProductoPagedBody")
            .RequireAuthorization("PRECIOSPRODUCTO.VIEW");

            group.MapGet("/{fleteCode}/{tipoCode}/{productoCode}/{monedaTipo}/{monedaCodigo}", async (
                string fleteCode, string tipoCode, string productoCode, string monedaTipo, string monedaCodigo,
                ISender sender) =>
            {
                var result = await sender.Send(new GetPrecioProductoQuery(fleteCode, tipoCode, productoCode, monedaTipo, monedaCodigo));
                return Results.Ok(result);
            })
            .WithName("GetPrecioProducto")
            .RequireAuthorization("PRECIOSPRODUCTO.VIEW");

            group.MapPut("/{fleteCode}/{tipoCode}/{productoCode}/{monedaTipo}/{monedaCodigo}", async (
                string fleteCode, string tipoCode, string productoCode, string monedaTipo, string monedaCodigo,
                UpdatePrecioProductoCommand command, ISender sender) =>
            {
                if (fleteCode != command.FleteCode || tipoCode != command.ProductoTipoCode ||
                    productoCode != command.ProductoCode || monedaTipo != command.CurrencyTypeCode ||
                    monedaCodigo != command.CurrencyCode)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePrecioProducto")
            .RequireAuthorization("PRECIOSPRODUCTO.UPDATE");

            group.MapDelete("/{fleteCode}/{tipoCode}/{productoCode}/{monedaTipo}/{monedaCodigo}", async (
                string fleteCode, string tipoCode, string productoCode, string monedaTipo, string monedaCodigo,
                ISender sender) =>
            {
                await sender.Send(new DeletePrecioProductoCommand(fleteCode, tipoCode, productoCode, monedaTipo, monedaCodigo));
                return Results.NoContent();
            })
            .WithName("DeletePrecioProducto")
            .RequireAuthorization("PRECIOSPRODUCTO.DELETE");
        }
    }
}
