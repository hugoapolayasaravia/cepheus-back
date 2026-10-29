using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.CreateCotizacionMetradoDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.DeleteCotizacionMetradoDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.GetCotizacionMetradoDetallesByResumen;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.UpdateCotizacionMetradoDetalle;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class CotizacionMetradoDetallesEndpoints
    {
        public static void MapCotizacionMetradoDetallesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/metrado/{nivel:int}/detalle")
                .WithTags("Cotizaciones - Metrado Detalle (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (
                string negocio, string anio, string mes, string codigo, int nivel,
                CreateCotizacionMetradoDetalleCommand command, ISender sender) =>
            {
                if (negocio != command.NegocioCode || anio != command.Year || mes != command.Month
                    || codigo != command.Code || nivel != command.LevelNumber)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/metrado/{nivel}/detalle/{result.Order}/{result.ProductoTipoCode}/{result.ProductoCode}",
                    result);
            })
            .WithName("CreateCotizacionVentaMetradoDetalle")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapGet("/", async (
                string negocio, string anio, string mes, string codigo, int nivel, ISender sender) =>
            {
                var result = await sender.Send(new GetCotizacionMetradoDetallesByResumenQuery(negocio, anio, mes, codigo, nivel));
                return Results.Ok(result);
            })
            .WithName("GetCotizacionVentaMetradoDetallesByResumen")
            .RequireAuthorization("COTIZACIONESVENTA.VIEW");

            group.MapPut("/{orden}/{tipoProducto}/{producto}", async (
                string negocio, string anio, string mes, string codigo, int nivel,
                string orden, string tipoProducto, string producto,
                UpdateCotizacionMetradoDetalleCommand command, ISender sender) =>
            {
                if (negocio != command.NegocioCode || codigo != command.Code || nivel != command.LevelNumber
                    || orden != command.Order || tipoProducto != command.ProductoTipoCode || producto != command.ProductoCode)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCotizacionVentaMetradoDetalle")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapDelete("/{orden}/{tipoProducto}/{producto}", async (
                string negocio, string anio, string mes, string codigo, int nivel,
                string orden, string tipoProducto, string producto, ISender sender) =>
            {
                await sender.Send(new DeleteCotizacionMetradoDetalleCommand(negocio, anio, mes, codigo, nivel, orden, tipoProducto, producto));
                return Results.NoContent();
            })
            .WithName("DeleteCotizacionVentaMetradoDetalle")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");
        }
    }
}
