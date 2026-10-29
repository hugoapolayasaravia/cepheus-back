using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.CreateCotizacionNota;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.DeleteCotizacionNota;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.GetCotizacionNotasByCotizacion;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class CotizacionNotasEndpoints
    {
        public static void MapCotizacionNotasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/notas")
                .WithTags("Cotizaciones - Notas (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (
                string negocio, string anio, string mes, string codigo,
                CreateCotizacionNotaCommand command, ISender sender) =>
            {
                if (negocio != command.NegocioCode || anio != command.Year || mes != command.Month || codigo != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/transacciones/cotizaciones/{negocio}/{anio}/{mes}/{codigo}/notas/{result.Sequence}",
                    result);
            })
            .WithName("CreateCotizacionVentaNota")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapGet("/", async (
                string negocio, string anio, string mes, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCotizacionNotasByCotizacionQuery(negocio, anio, mes, codigo));
                return Results.Ok(result);
            })
            .WithName("GetCotizacionVentaNotasByCotizacion")
            .RequireAuthorization("COTIZACIONESVENTA.VIEW");

            group.MapDelete("/{sequence:int}", async (
                string negocio, string anio, string mes, string codigo, int sequence, ISender sender) =>
            {
                await sender.Send(new DeleteCotizacionNotaCommand(negocio, anio, mes, codigo, sequence));
                return Results.NoContent();
            })
            .WithName("DeleteCotizacionVentaNota")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");
        }
    }
}
