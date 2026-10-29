using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.CreateArticuloProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.DeleteArticuloProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedoresByArticulo;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedoresByProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.UpdateArticuloProveedor;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ArticuloProveedoresEndpoints
    {
        public static void MapArticuloProveedoresEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/articulo-proveedor")
                .WithTags("Artículo-Proveedor (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateArticuloProveedorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/maestros/articulo-proveedor/{result.PlantaCode}/{result.ArticuloCode}/{result.ProveedorCode}",
                    result);
            })
            .WithName("CreateArticuloProveedor")
            .RequireAuthorization("ARTICULOS.UPDATE");

            group.MapGet("/articulo/{codigoArticulo}", async (string codigoArticulo, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloProveedoresByArticuloQuery(codigoArticulo));
                return Results.Ok(result);
            })
            .WithName("GetArticuloProveedoresByArticulo")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapGet("/proveedor/{codigoProveedor}", async (string codigoProveedor, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloProveedoresByProveedorQuery(codigoProveedor));
                return Results.Ok(result);
            })
            .WithName("GetArticuloProveedoresByProveedor")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapGet("/{codigoPlanta}/{codigoArticulo}/{codigoProveedor}", async (
                string codigoPlanta, string codigoArticulo, string codigoProveedor, ISender sender) =>
            {
                var result = await sender.Send(new GetArticuloProveedorQuery(codigoPlanta, codigoArticulo, codigoProveedor));
                return Results.Ok(result);
            })
            .WithName("GetArticuloProveedor")
            .RequireAuthorization("ARTICULOS.VIEW");

            group.MapPut("/{codigoPlanta}/{codigoArticulo}/{codigoProveedor}", async (
                string codigoPlanta, string codigoArticulo, string codigoProveedor,
                UpdateArticuloProveedorCommand command, ISender sender) =>
            {
                if (codigoPlanta != command.PlantaCode || codigoArticulo != command.ArticuloCode || codigoProveedor != command.ProveedorCode)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateArticuloProveedor")
            .RequireAuthorization("ARTICULOS.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoArticulo}/{codigoProveedor}", async (
                string codigoPlanta, string codigoArticulo, string codigoProveedor, ISender sender) =>
            {
                await sender.Send(new DeleteArticuloProveedorCommand(codigoPlanta, codigoArticulo, codigoProveedor));
                return Results.NoContent();
            })
            .WithName("DeleteArticuloProveedor")
            .RequireAuthorization("ARTICULOS.UPDATE");
        }
    }
}
