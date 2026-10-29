using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ActivarOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CopiarOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CreateOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ExportarHojasCorteOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetOrdenesTrabajoVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetOrdenTrabajoVentaByCode;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.InactivarOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.CreateOrdenTrabajoVentaDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.DeleteOrdenTrabajoVentaDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.OrdenTrabajoVentaDetalles.UpdateOrdenTrabajoVentaDetalle;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.UpdateOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularAprobacionOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobacionEspecialOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobarOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.BloquearOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.DesbloquearOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetCreditoOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetSiguienteDocumentoOrdenTrabajoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.PasarAProduccionOrdenTrabajoVenta;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones;

public static class OrdenesTrabajoVentaEndpoints
{
    private const string Base = "/api/facturacion/transacciones/ordenes-trabajo-venta";

    public static void MapOrdenesTrabajoVentaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Base)
            .WithTags("Órdenes de Trabajo de Ventas (Facturación)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateOrdenTrabajoVentaCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"{Base}/{result.Code}", result);
        })
        .WithName("CreateOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.CREATE");

        // Búsqueda: parámetros literales de ges_ordenTrabajo.
        group.MapGet("/paged", async ([AsParameters] GetOrdenesTrabajoVentaPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetOrdenesTrabajoVentaPagedQueryString")
        .RequireAuthorization("ORDENESTRABAJOVENTA.VIEW");

        group.MapPost("/paged/body", async (GetOrdenesTrabajoVentaPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetOrdenesTrabajoVentaPagedBody")
        .RequireAuthorization("ORDENESTRABAJOVENTA.VIEW");

        group.MapGet("/{code}", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new GetOrdenTrabajoVentaByCodeQuery(code))))
        .WithName("GetOrdenTrabajoVentaByCode")
        .RequireAuthorization("ORDENESTRABAJOVENTA.VIEW");

        group.MapPut("/{code}", async (string code, OrdenTrabajoVentaUpdateFields body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateOrdenTrabajoVentaCommand(code, body))))
        .WithName("UpdateOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.UPDATE");

        group.MapPost("/{code}/copiar", async (string code, ISender sender) =>
        {
            var result = await sender.Send(new CopiarOrdenTrabajoVentaCommand(code));
            return Results.Created($"{Base}/{result.Code}", result);
        })
        .WithName("CopiarOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.CREATE");

        // Anulación: DELETE con cuerpo (motivo).
        group.MapDelete("/{code}", async (string code, [FromBody] AnularOrdenTrabajoVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new AnularOrdenTrabajoVentaCommand(code, body))))
        .WithName("AnularOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.ANNUL");

        group.MapPost("/{code}/activar", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new ActivarOrdenTrabajoVentaCommand(code))))
        .WithName("ActivarOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.ACTIVATE");

        group.MapPost("/{code}/inactivar", async (string code, InactivarOrdenTrabajoVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new InactivarOrdenTrabajoVentaCommand(code, body))))
        .WithName("InactivarOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.ACTIVATE");

        group.MapPost("/{code}/hojas-corte", async (string code, ExportarHojasCorteRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new ExportarHojasCorteOrdenTrabajoVentaCommand(code, body))))
        .WithName("ExportarHojasCorteOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.UPDATE");

        // ---------------------------------------------------------------- líneas

        group.MapPost("/{code}/detalles", async (string code, OrdenTrabajoVentaDetalleRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new CreateOrdenTrabajoVentaDetalleCommand(code, body))))
        .WithName("CreateOrdenTrabajoVentaDetalle")
        .RequireAuthorization("ORDENESTRABAJOVENTA.UPDATE");

        group.MapPut("/{code}/detalles/{item:int}", async (
            string code,
            int item,
            UpdateOrdenTrabajoVentaDetalleRequest body,
            ISender sender) =>
            Results.Ok(await sender.Send(new UpdateOrdenTrabajoVentaDetalleCommand(code, item, body))))
        .WithName("UpdateOrdenTrabajoVentaDetalle")
        .RequireAuthorization("ORDENESTRABAJOVENTA.UPDATE");

        group.MapDelete("/{code}/detalles/{item:int}", async (string code, int item, ISender sender) =>
            Results.Ok(await sender.Send(new DeleteOrdenTrabajoVentaDetalleCommand(code, item))))
        .WithName("DeleteOrdenTrabajoVentaDetalle")
        .RequireAuthorization("ORDENESTRABAJOVENTA.UPDATE");

        // ---------------------------------------------------------------- aprobación

        // Vista previa de la numeración de la planta (no reserva el número).
        group.MapGet("/documentos/siguiente", async (string planta, TipoDocumentoAprobacion tipo, ISender sender) =>
            Results.Ok(await sender.Send(new GetSiguienteDocumentoQuery(planta, tipo))))
        .WithName("GetSiguienteDocumentoOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.VIEW");

        // Línea de crédito del cliente/obra frente al total de la OT.
        group.MapGet("/{code}/credito", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new GetCreditoOrdenTrabajoVentaQuery(code))))
        .WithName("GetCreditoOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.VIEW");

        // Bloquea para aprobación: crédito se aprueba solo, contado queda pendiente de Créditos.
        group.MapPost("/{code}/bloquear", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new BloquearOrdenTrabajoVentaCommand(code))))
        .WithName("BloquearOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.LOCK");

        group.MapPost("/{code}/desbloquear", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new DesbloquearOrdenTrabajoVentaCommand(code))))
        .WithName("DesbloquearOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.LOCK");

        group.MapPost("/{code}/aprobar", async (string code, AprobarOrdenTrabajoVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new AprobarOrdenTrabajoVentaCommand(code, body))))
        .WithName("AprobarOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.APPROVE");

        group.MapPost("/{code}/aprobacion-especial", async (string code, AprobacionEspecialOrdenTrabajoVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new AprobacionEspecialOrdenTrabajoVentaCommand(code, body))))
        .WithName("AprobacionEspecialOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.SPECIAL_APPROVAL");

        group.MapDelete("/{code}/aprobacion", async (string code, [FromBody] AnularAprobacionOrdenTrabajoVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new AnularAprobacionOrdenTrabajoVentaCommand(code, body))))
        .WithName("AnularAprobacionOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.ANNUL_APPROVAL");

        group.MapPost("/{code}/pase-produccion", async (string code, ISender sender) =>
            Results.Ok(await sender.Send(new PasarAProduccionOrdenTrabajoVentaCommand(code))))
        .WithName("PasarAProduccionOrdenTrabajoVenta")
        .RequireAuthorization("ORDENESTRABAJOVENTA.PRODUCTION");
    }
}
