using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.ChangeEstadoOrdenServicio;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.CreateOrdenServicio;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.GetOrdenesServicioPaginated;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.GetOrdenServicioByCode;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.CreateOrdenServicioDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.DeleteOrdenServicioDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.UpdateOrdenServicioDetalle;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.ProcesarOrdenServicio;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.UpdateOrdenServicio;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Transacciones;

public static class OrdenesServicioEndpoints
{
    public static void MapOrdenesServicioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/logistica/transacciones/ordenes-servicio")
            .WithTags("Órdenes de Servicio (Logística)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateOrdenServicioCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created(
                $"/api/logistica/transacciones/ordenes-servicio/{result.PlantaCode}/{result.Code}", result);
        })
        .WithName("CreateOrdenServicio")
        .RequireAuthorization("ORDENESSERVICIO.CREATE");

        // Listado: parámetros literales de Logi_sp_Listado_MOrdenServicio_I (por query string o por body).
        group.MapGet("/paged", async ([AsParameters] GetOrdenesServicioPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetOrdenesServicioPagedQueryString")
        .RequireAuthorization("ORDENESSERVICIO.VIEW");

        group.MapPost("/paged/body", async (GetOrdenesServicioPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetOrdenesServicioPagedBody")
        .RequireAuthorization("ORDENESSERVICIO.VIEW");

        group.MapGet("/{codigoPlanta}/{codigo}", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new GetOrdenServicioByCodeQuery(codigoPlanta, codigo))))
        .WithName("GetOrdenServicioByCode")
        .RequireAuthorization("ORDENESSERVICIO.VIEW");

        group.MapPut("/{codigoPlanta}/{codigo}", async (
            string codigoPlanta, string codigo, UpdateOrdenServicioRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateOrdenServicioCommand
            {
                PlantaCode = codigoPlanta,
                Code = codigo,
                ComprobantePagoCode = body.ComprobantePagoCode,
                NumeroDocumento = body.NumeroDocumento,
                ProveedorCode = body.ProveedorCode,
                MonedaCode = body.MonedaCode,
                FormaPagoCode = body.FormaPagoCode,
                FechaEmision = body.FechaEmision,
                FechaRecepcion = body.FechaRecepcion,
                CompradorCode = body.CompradorCode,
                LugarEnvioCode = body.LugarEnvioCode,
                TramiteCode = body.TramiteCode,
                NotaCompraCode = body.NotaCompraCode,
                UnidadNegocioCode = body.UnidadNegocioCode,
                TrabajadorCode = body.TrabajadorCode,
                Observaciones1 = body.Observaciones1,
                Observaciones2 = body.Observaciones2,
                Igv = body.Igv,
                NoGravable = body.NoGravable,
                Renta = body.Renta,
                Fonavi = body.Fonavi,
                Servicio = body.Servicio,
                IgvExterior = body.IgvExterior
            })))
        .WithName("UpdateOrdenServicio")
        .RequireAuthorization("ORDENESSERVICIO.UPDATE");

        // Aprobar (valida contra la matriz), cerrar o anular. Mismo permiso UPDATE que OC / Pedidos / Nota de Ingreso.
        group.MapPatch("/{codigoPlanta}/{codigo}/estado", async (
            string codigoPlanta, string codigo, ChangeEstadoOrdenServicioBody body, ISender sender) =>
            Results.Ok(await sender.Send(new ChangeEstadoOrdenServicioCommand(codigoPlanta, codigo, body.NuevoEstado))))
        .WithName("ChangeEstadoOrdenServicio")
        .RequireAuthorization("ORDENESSERVICIO.UPDATE");

        // Procesar: actualiza costos de stock y materiales de la OT.
        group.MapPost("/{codigoPlanta}/{codigo}/procesar", async (string codigoPlanta, string codigo, ISender sender) =>
            Results.Ok(await sender.Send(new ProcesarOrdenServicioCommand(codigoPlanta, codigo))))
        .WithName("ProcesarOrdenServicio")
        .RequireAuthorization("ORDENESSERVICIO.UPDATE");

        // Líneas de detalle (solo con la orden Pendiente).
        group.MapPost("/{codigoPlanta}/{codigo}/detalles", async (
            string codigoPlanta, string codigo, OrdenServicioDetalleRequest body, ISender sender) =>
        {
            var result = await sender.Send(new CreateOrdenServicioDetalleCommand
            {
                PlantaCode = codigoPlanta,
                OrdenServicioCode = codigo,
                Detalle = body
            });
            return Results.Created($"/api/logistica/transacciones/ordenes-servicio/{codigoPlanta}/{codigo}", result);
        })
        .WithName("CreateOrdenServicioDetalle")
        .RequireAuthorization("ORDENESSERVICIO.UPDATE");

        group.MapPut("/{codigoPlanta}/{codigo}/detalles/{item:int}", async (
            string codigoPlanta, string codigo, int item, OrdenServicioDetalleRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateOrdenServicioDetalleCommand
            {
                PlantaCode = codigoPlanta,
                OrdenServicioCode = codigo,
                ItemNumber = item,
                Detalle = body
            })))
        .WithName("UpdateOrdenServicioDetalle")
        .RequireAuthorization("ORDENESSERVICIO.UPDATE");

        group.MapDelete("/{codigoPlanta}/{codigo}/detalles/{item:int}", async (
            string codigoPlanta, string codigo, int item, ISender sender) =>
            Results.Ok(await sender.Send(new DeleteOrdenServicioDetalleCommand(codigoPlanta, codigo, item))))
        .WithName("DeleteOrdenServicioDetalle")
        .RequireAuthorization("ORDENESSERVICIO.UPDATE");
    }

    public sealed record ChangeEstadoOrdenServicioBody(string NuevoEstado);

    /// <summary>Cuerpo de la modificación de cabecera (la clave planta/código va en la ruta).</summary>
    public sealed record UpdateOrdenServicioRequest(
        string ComprobantePagoCode,
        string? NumeroDocumento,
        string ProveedorCode,
        string MonedaCode,
        string FormaPagoCode,
        DateTime FechaEmision,
        DateTime FechaRecepcion,
        string CompradorCode,
        string LugarEnvioCode,
        string TramiteCode,
        string? NotaCompraCode,
        string UnidadNegocioCode,
        string TrabajadorCode,
        string? Observaciones1,
        string? Observaciones2,
        decimal Igv,
        decimal NoGravable,
        decimal Renta,
        decimal Fonavi,
        decimal Servicio,
        decimal IgvExterior);
}
