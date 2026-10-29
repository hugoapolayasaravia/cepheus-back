// Cepheus.API/Endpoints/Logistica/Transacciones/CotizacionesEndpoints.cs
using Cepheus.Application.Features.Logistica.Transacciones.CotizacionDetalles.CreateCotizacionDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.CotizacionDetalles.DeleteCotizacionDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.DescartarProveedor;
using Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.InvitarProveedor;
using Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.RegistrarRespuestaProveedor;
using Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.SeleccionarProveedor;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.ChangeEstadoCotizacion;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.CopyCotizacion;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.CreateCotizacion;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GetCotizacionByCode;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GetCotizacionesPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.UpdateCotizacion;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones
{
    public static class CotizacionesEndpoints
    {
        public static void MapCotizacionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/transacciones/cotizaciones")
                .WithTags("Cotizaciones (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCotizacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/transacciones/cotizaciones/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CreateCotizacion")
            .RequireAuthorization("COTIZACIONES.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetCotizacionesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetCotizacionesPagedQueryString")
            .RequireAuthorization("COTIZACIONES.VIEW");

            group.MapPost("/paged/body", async (GetCotizacionesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetCotizacionesPagedBody")
            .RequireAuthorization("COTIZACIONES.VIEW");

            group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetCotizacionByCodeQuery(codigoPlanta, codigo))))
            .WithName("GetCotizacionByCode")
            .RequireAuthorization("COTIZACIONES.VIEW");

            group.MapPut("/{codigoPlanta}/{codigo}", async (
                string codigoPlanta, string codigo, UpdateCotizacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateCotizacion")
            .RequireAuthorization("COTIZACIONES.UPDATE");

            group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
                string codigoPlanta, string codigo, ChangeEstadoBody body, ISender sender) =>
                Results.Ok(await sender.Send(new ChangeEstadoCotizacionCommand(codigoPlanta, codigo, body.NuevoEstado))))
            .WithName("ChangeEstadoCotizacion")
            .RequireAuthorization("COTIZACIONES.UPDATE");

            group.MapPost("/{codigoPlanta}/{codigo}/copiar", async (
                string codigoPlanta, string codigo, CopyCotizacionBody body, ISender sender) =>
            {
                var result = await sender.Send(new CopyCotizacionCommand(codigoPlanta, codigo, body.NuevaFechaLimite));
                return Results.Created($"/api/logistica/transacciones/cotizaciones/{result.PlantaCode}/{result.Code}", result);
            })
            .WithName("CopyCotizacion")
            .RequireAuthorization("COTIZACIONES.CREATE");

            // Detalle de artículos — reusa permisos de Cotizaciones
            group.MapPost("/{codigoPlanta}/{codigoCotizacion}/detalles", async (
                string codigoPlanta, string codigoCotizacion, CreateCotizacionDetalleCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoPlanta, command.PlantaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigoCotizacion, command.CotizacionCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("CreateCotizacionDetalle")
            .RequireAuthorization("COTIZACIONES.UPDATE");

            group.MapDelete("/{codigoPlanta}/{codigoCotizacion}/detalles/{codigoArticulo}", async (
                string codigoPlanta, string codigoCotizacion, string codigoArticulo, ISender sender) =>
                Results.Ok(await sender.Send(new DeleteCotizacionDetalleCommand(codigoPlanta, codigoCotizacion, codigoArticulo))))
            .WithName("DeleteCotizacionDetalle")
            .RequireAuthorization("COTIZACIONES.UPDATE");

            // Proveedores — reusa permisos de Cotizaciones
            group.MapPost("/{codigoPlanta}/{codigoCotizacion}/proveedores", async (
                string codigoPlanta, string codigoCotizacion, InvitarProveedorBody body, ISender sender) =>
            {
                var command = new InvitarProveedorCommand(codigoPlanta, codigoCotizacion, body.ProveedorCode, body.MonedaCode, body.Observaciones);
                return Results.Ok(await sender.Send(command));
            })
            .WithName("InvitarProveedor")
            .RequireAuthorization("COTIZACIONES.UPDATE");

            group.MapPost("/{codigoPlanta}/{codigoCotizacion}/proveedores/{codigoProveedor}/respuesta", async (
                string codigoPlanta, string codigoCotizacion, string codigoProveedor,
                RegistrarRespuestaBody body, ISender sender) =>
            {
                var command = new RegistrarRespuestaProveedorCommand(codigoPlanta, codigoCotizacion, codigoProveedor, body.Lineas);
                return Results.Ok(await sender.Send(command));
            })
            .WithName("RegistrarRespuestaProveedor")
            .RequireAuthorization("COTIZACIONES.UPDATE");

            group.MapPatch("/{codigoPlanta}/{codigoCotizacion}/proveedores/{codigoProveedor}/seleccionar", async (
                string codigoPlanta, string codigoCotizacion, string codigoProveedor, ISender sender) =>
                Results.Ok(await sender.Send(new SeleccionarProveedorCommand(codigoPlanta, codigoCotizacion, codigoProveedor))))
            .WithName("SeleccionarProveedor")
            .RequireAuthorization("COTIZACIONES.UPDATE");

            group.MapPatch("/{codigoPlanta}/{codigoCotizacion}/proveedores/{codigoProveedor}/descartar", async (
                string codigoPlanta, string codigoCotizacion, string codigoProveedor, ISender sender) =>
                Results.Ok(await sender.Send(new DescartarProveedorCommand(codigoPlanta, codigoCotizacion, codigoProveedor))))
            .WithName("DescartarProveedor")
            .RequireAuthorization("COTIZACIONES.UPDATE");
        }

        public record ChangeEstadoBody(string NuevoEstado);
        public record CopyCotizacionBody(DateTime NuevaFechaLimite);
        public record InvitarProveedorBody(string ProveedorCode, string MonedaCode, string? Observaciones);
        public record RegistrarRespuestaBody(List<RegistrarRespuestaLineaInput> Lineas);
    }
}