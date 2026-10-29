using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.CreateDocumentoCompra;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.GetDocumentoCompraByKey;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.GetDocumentosCompraPaginated;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.UpdateDocumentoCompra;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.CreateDocumentoCompraDetalle;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.DeleteDocumentoCompraDetalle;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.UpdateDocumentoCompraDetalle;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class DocumentosCompraEndpoints
    {
        public static void MapDocumentosCompraEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/transacciones/documentos-compra")
                .WithTags("Documentos de compra (Contabilidad)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateDocumentoCompraCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/contabilidad/transacciones/documentos-compra/{result.ComprobantePagoCode}/{result.ProveedorCode}/{result.NumeroDocumento}",
                    result);
            })
            .WithName("CreateDocumentoCompra")
            .RequireAuthorization("DOCUMENTOSCOMPRA.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetDocumentosCompraPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetDocumentosCompraPagedQueryString")
            .RequireAuthorization("DOCUMENTOSCOMPRA.VIEW");

            group.MapPost("/paged/body", async (GetDocumentosCompraPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetDocumentosCompraPagedBody")
            .RequireAuthorization("DOCUMENTOSCOMPRA.VIEW");

            group.MapGet("/{comprobantePagoCode}/{proveedorCode}/{numeroDocumento}", async (
                string comprobantePagoCode, string proveedorCode, string numeroDocumento, ISender sender) =>
                Results.Ok(await sender.Send(new GetDocumentoCompraByKeyQuery(comprobantePagoCode, proveedorCode, numeroDocumento))))
            .WithName("GetDocumentoCompraByKey")
            .RequireAuthorization("DOCUMENTOSCOMPRA.VIEW");

            group.MapPut("/{comprobantePagoCode}/{proveedorCode}/{numeroDocumento}", async (
                string comprobantePagoCode, string proveedorCode, string numeroDocumento,
                UpdateDocumentoCompraCommand command, ISender sender) =>
            {
                if (!string.Equals(comprobantePagoCode, command.ComprobantePagoCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(proveedorCode, command.ProveedorCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(numeroDocumento, command.NumeroDocumento, StringComparison.Ordinal))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateDocumentoCompra")
            .RequireAuthorization("DOCUMENTOSCOMPRA.UPDATE");

            group.MapPost("/{comprobantePagoCode}/{proveedorCode}/{numeroDocumento}/detalles", async (
                string comprobantePagoCode, string proveedorCode, string numeroDocumento,
                CreateDocumentoCompraDetalleCommand command, ISender sender) =>
            {
                if (!KeyMatches(comprobantePagoCode, proveedorCode, numeroDocumento,
                        command.ComprobantePagoCode, command.ProveedorCode, command.NumeroDocumento))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("CreateDocumentoCompraDetalle")
            .RequireAuthorization("DOCUMENTOSCOMPRA.UPDATE");

            group.MapPut("/{comprobantePagoCode}/{proveedorCode}/{numeroDocumento}/detalles/{item:int}", async (
                string comprobantePagoCode, string proveedorCode, string numeroDocumento, int item,
                UpdateDocumentoCompraDetalleCommand command, ISender sender) =>
            {
                if (!KeyMatches(comprobantePagoCode, proveedorCode, numeroDocumento,
                        command.ComprobantePagoCode, command.ProveedorCode, command.NumeroDocumento)
                    || item != command.ItemNumber)
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateDocumentoCompraDetalle")
            .RequireAuthorization("DOCUMENTOSCOMPRA.UPDATE");

            group.MapDelete("/{comprobantePagoCode}/{proveedorCode}/{numeroDocumento}/detalles/{item:int}", async (
                string comprobantePagoCode, string proveedorCode, string numeroDocumento, int item, ISender sender) =>
                Results.Ok(await sender.Send(
                    new DeleteDocumentoCompraDetalleCommand(comprobantePagoCode, proveedorCode, numeroDocumento, item))))
            .WithName("DeleteDocumentoCompraDetalle")
            .RequireAuthorization("DOCUMENTOSCOMPRA.UPDATE");
        }

        private static bool KeyMatches(
            string routeTdo, string routePrv, string routeNumero,
            string bodyTdo, string bodyPrv, string bodyNumero)
            => string.Equals(routeTdo, bodyTdo, StringComparison.OrdinalIgnoreCase)
               && string.Equals(routePrv, bodyPrv, StringComparison.OrdinalIgnoreCase)
               && string.Equals(routeNumero, bodyNumero, StringComparison.Ordinal);
    }
}
