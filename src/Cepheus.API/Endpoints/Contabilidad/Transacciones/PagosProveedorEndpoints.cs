using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarComprobanteRetencion;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarPagoRealPagoProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.AnularPagoProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.MarcarImpresionPagoProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ComprobanteCajaBancosPagoProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ComprobanteRetencionPagoProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.DocumentosPorPagar;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ListadoPagosProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.CreatePagoProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.DocumentosPendientes.GetDocumentosPendientesProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GetPagoProveedorByCode;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GenerarAsientoPagoProveedor;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GetPagosProveedorPaginated;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.SetPagoProveedorDocumentos;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.UpdatePagoProveedor;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class PagosProveedorEndpoints
    {
        public static void MapPagosProveedorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/transacciones/pagos-proveedor")
                .WithTags("Pagos a proveedores")
                .RequireAuthorization();

            // Legacy com_con_documentos: documentos pendientes del proveedor (solo soles).
            group.MapGet("/documentos-pendientes", async (string proveedorCode, ISender sender) =>
            {
                var result = await sender.Send(new GetDocumentosPendientesProveedorQuery(proveedorCode));
                return Results.Ok(result);
            })
            .WithName("GetDocumentosPendientesProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");

            group.MapPost("/", async (CreatePagoProveedorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/transacciones/pagos-proveedor/{result.Code}", result);
            })
            .WithName("CreatePagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.CREATE");

            // Legacy com_con_cheques.
            group.MapGet("/paged", async (
                [AsParameters] GetPagosProveedorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPagosProveedorPagedQueryString")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");

            group.MapPost("/paged/body", async (
                GetPagosProveedorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetPagosProveedorPagedBody")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");

            group.MapGet("/{code}", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GetPagoProveedorByCodeQuery(code));
                return Results.Ok(result);
            })
            .WithName("GetPagoProveedorByCode")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");

            group.MapPut("/{code}", async (string code, UpdatePagoProveedorCommand command, ISender sender) =>
            {
                if (!string.Equals(code, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdatePagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.UPDATE");

            // Documentos que cancela el pago (reemplaza el detalle y recalcula retención y saldos).
            group.MapPut("/{code}/documentos", async (string code, SetPagoProveedorDocumentosCommand command, ISender sender) =>
            {
                if (!string.Equals(code, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("SetPagoProveedorDocumentos")
            .RequireAuthorization("PAGOSPROVEEDOR.UPDATE");

            // Legacy ue_set_btn_generar: asiento de cancelación y comprobante de retención.
            group.MapPost("/{code}/asiento", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new GenerarAsientoPagoProveedorCommand(code));
                return Results.Ok(result);
            })
            .WithName("GenerarAsientoPagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.UPDATE");

            // Legacy ue_set_btn_eli_bor: anulación de la operación, del asiento o liberación del cheque.
            group.MapPost("/{code}/anulacion", async (string code, AnularPagoProveedorBody body, ISender sender) =>
            {
                var result = await sender.Send(new AnularPagoProveedorCommand(
                    code, body.AnularOperacion, body.AnularAsiento, body.LiberarCheque));
                return Results.Ok(result);
            })
            .WithName("AnularPagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.UPDATE");

            // Legacy w_com_pagoreal_cheques.
            group.MapPut("/{code}/pago-real", async (string code, ActualizarPagoRealBody body, ISender sender) =>
            {
                var result = await sender.Send(new ActualizarPagoRealPagoProveedorCommand(code, body.PagoRealSol));
                return Results.Ok(result);
            })
            .WithName("ActualizarPagoRealPagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.UPDATE");

            // Legacy w_comprobante.
            group.MapPut("/{code}/comprobante-retencion", async (string code, ActualizarComprobanteBody body, ISender sender) =>
            {
                var result = await sender.Send(new ActualizarComprobanteRetencionCommand(
                    code, body.NumeroComprobanteRetencion, body.FechaComprobanteRetencion));
                return Results.Ok(result);
            })
            .WithName("ActualizarComprobanteRetencion")
            .RequireAuthorization("PAGOSPROVEEDOR.UPDATE");

            // Legacy w_com_rep_cheques: registra la impresión del comprobante.
            group.MapPost("/{code}/impresion", async (string code, MarcarImpresionBody body, ISender sender) =>
            {
                var result = await sender.Send(new MarcarImpresionPagoProveedorCommand(code, body.Tipo));
                return Results.Ok(result);
            })
            .WithName("MarcarImpresionPagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.UPDATE");

            // Reportes (solo soles).
            // Legacy com_Listado_Cheques_Proveedores.
            group.MapGet("/reportes/listado", async (
                [AsParameters] ListadoPagosProveedorQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("ListadoPagosProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");

            // Legacy ad_rpt_com_docxpagar.
            group.MapGet("/reportes/documentos-por-pagar", async (
                [AsParameters] DocumentosPorPagarQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("DocumentosPorPagar")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");

            // Legacy ad_rpt_com_retenciones.
            group.MapGet("/{code}/reportes/retencion", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new ComprobanteRetencionPagoProveedorQuery(code));
                return Results.Ok(result);
            })
            .WithName("ComprobanteRetencionPagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");

            // Legacy ad_rpt_cajabancos.
            group.MapGet("/{code}/reportes/caja-bancos", async (string code, ISender sender) =>
            {
                var result = await sender.Send(new ComprobanteCajaBancosPagoProveedorQuery(code));
                return Results.Ok(result);
            })
            .WithName("ComprobanteCajaBancosPagoProveedor")
            .RequireAuthorization("PAGOSPROVEEDOR.VIEW");
        }
    }

    public record AnularPagoProveedorBody(bool AnularOperacion, bool AnularAsiento, bool LiberarCheque);

    public record ActualizarPagoRealBody(decimal PagoRealSol);

    public record ActualizarComprobanteBody(string NumeroComprobanteRetencion, DateTime? FechaComprobanteRetencion);

    public record MarcarImpresionBody(string Tipo);
}
