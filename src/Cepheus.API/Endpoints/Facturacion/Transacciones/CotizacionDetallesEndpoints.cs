using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.CreateCotizacionDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.DeleteCotizacionDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.GetCotizacionDetallesByCotizacion;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.UpdateCotizacionDetalle;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class CotizacionDetallesEndpoints
    {
        public static void MapCotizacionDetallesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/detalles")
                .WithTags("Cotizaciones - Detalle (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (
                string negocio, string anio, string mes, string codigo,
                CreateCotizacionDetalleCommand command, ISender sender) =>
            {
                if (negocio != command.NegocioCode || anio != command.Year || mes != command.Month || codigo != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/detalles/{result.Item}",
                    result);
            })
            .WithName("CreateCotizacionVentaDetalle")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapGet("/", async (
                string negocio, string anio, string mes, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCotizacionDetallesByCotizacionQuery(negocio, anio, mes, codigo));
                return Results.Ok(result);
            })
            .WithName("GetCotizacionVentaDetallesByCotizacion")
            .RequireAuthorization("COTIZACIONESVENTA.VIEW");

            group.MapPut("/{item:int}", async (
                string negocio, string anio, string mes, string codigo, int item,
                UpdateCotizacionDetalleCommand command, ISender sender) =>
            {
                if (item != command.Item || negocio != command.NegocioCode || codigo != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCotizacionVentaDetalle")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapDelete("/{item:int}", async (
                string negocio, string anio, string mes, string codigo, int item, ISender sender) =>
            {
                await sender.Send(new DeleteCotizacionDetalleCommand(negocio, anio, mes, codigo, item));
                return Results.NoContent();
            })
            .WithName("DeleteCotizacionVentaDetalle")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");
        }
    }
}
