using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.AnularCotizacion;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.ApproveCotizacion;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.ChangeCotizacionEstado;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.CreateCotizacion;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.GetCotizacionByCode;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.GetCotizacionesPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.UpdateCotizacion;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class CotizacionesEndpoints
    {
        public static void MapCotizacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/transacciones/cotizaciones")
                .WithTags("Cotizaciones (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCotizacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/transacciones/cotizaciones/{result.NegocioCode}/{result.Year}/{result.Month}/{result.Code}",
                    result);
            })
            .WithName("CreateCotizacionVenta")
            .RequireAuthorization("COTIZACIONESVENTA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCotizacionesPaginatedQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCotizacionesVentaPagedQueryString")
            .RequireAuthorization("COTIZACIONESVENTA.VIEW");

            group.MapPost("/paged/body", async (
                GetCotizacionesPaginatedQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCotizacionesVentaPagedBody")
            .RequireAuthorization("COTIZACIONES.VIEW");

            group.MapGet("/{negocio}/{anio}/{mes}/{codigo}", async (
                string negocio, string anio, string mes, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCotizacionByCodeQuery(negocio, anio, mes, codigo));
                return Results.Ok(result);
            })
            .WithName("GetCotizacionVentaByCode")
            .RequireAuthorization("COTIZACIONESVENTA.VIEW");

            group.MapPut("/{negocio}/{anio}/{mes}/{codigo}", async (
                string negocio, string anio, string mes, string codigo,
                UpdateCotizacionCommand command, ISender sender) =>
            {
                if (negocio != command.NegocioCode || anio != command.Year || mes != command.Month || codigo != command.Code)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCotizacionVenta")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");

            group.MapPatch("/{negocio}/{anio}/{mes}/{codigo}/aprobar", async (
                string negocio, string anio, string mes, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new ApproveCotizacionCommand(negocio, anio, mes, codigo));
                return Results.Ok(result);
            })
            .WithName("ApproveCotizacionVenta")
            .RequireAuthorization("COTIZACIONESVENTA.APPROVE");

            group.MapPatch("/{negocio}/{anio}/{mes}/{codigo}/anular", async (
                string negocio, string anio, string mes, string codigo,
                AnularCotizacionRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new AnularCotizacionCommand(negocio, anio, mes, codigo, body.Reason));
                return Results.Ok(result);
            })
            .WithName("AnularCotizacionVenta")
            .RequireAuthorization("COTIZACIONESVENTA.CANCEL");

            group.MapPatch("/{negocio}/{anio}/{mes}/{codigo}/estado", async (
                string negocio, string anio, string mes, string codigo,
                ChangeCotizacionEstadoRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new ChangeCotizacionEstadoCommand(negocio, anio, mes, codigo, body.NuevoEstado));
                return Results.Ok(result);
            })
            .WithName("ChangeCotizacionVentaEstado")
            .RequireAuthorization("COTIZACIONESVENTA.UPDATE");
        }

        public record AnularCotizacionRequestBody(string Reason);
        public record ChangeCotizacionEstadoRequestBody(string NuevoEstado);
    }
}
