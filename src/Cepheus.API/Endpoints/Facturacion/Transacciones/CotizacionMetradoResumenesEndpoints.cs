using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.CreateCotizacionMetradoResumen;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.DeleteCotizacionMetradoResumen;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.GetCotizacionMetradoResumenesByCotizacion;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.UpdateCotizacionMetradoResumen;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class CotizacionMetradoResumenesEndpoints
    {
        public static void MapCotizacionMetradoResumenesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/metrado")
                .WithTags("Cotizaciones - Metrado Resumen (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (
                string negocio, string anio, string mes, string codigo,
                CreateCotizacionMetradoResumenCommand command, ISender sender) =>
            {
                if (negocio != command.NegocioCode || anio != command.Year || mes != command.Month || codigo != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/metrado/{result.LevelNumber}",
                    result);
            })
            .WithName("CreateCotizacionVentaMetradoResumen")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapGet("/", async (
                string negocio, string anio, string mes, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCotizacionMetradoResumenesByCotizacionQuery(negocio, anio, mes, codigo));
                return Results.Ok(result);
            })
            .WithName("GetCotizacionMetradoVentaResumenesByCotizacion")
            .RequireAuthorization("COTIZACIONESVENTA.VIEW");

            group.MapPut("/{nivel:int}", async (
                string negocio, string anio, string mes, string codigo, int nivel,
                UpdateCotizacionMetradoResumenCommand command, ISender sender) =>
            {
                if (nivel != command.LevelNumber || negocio != command.NegocioCode || codigo != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCotizacionVentaMetradoResumen")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapDelete("/{nivel:int}", async (
                string negocio, string anio, string mes, string codigo, int nivel, ISender sender) =>
            {
                await sender.Send(new DeleteCotizacionMetradoResumenCommand(negocio, anio, mes, codigo, nivel));
                return Results.NoContent();
            })
            .WithName("DeleteCotizacionVentaMetradoResumen")
            .RequireAuthorization("COTIZACCOTIZACIONESVENTAIONES.UPDATE");
        }
    }
}
