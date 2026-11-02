using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AddDocumentosCancelacionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AnularAsientoCancelacionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CreateCancelacionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CuadrarCancelacionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.DeleteCancelacionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GenerarAsientoCancelacionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GetCancelacionesVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GetCancelacionVentaByCode;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.RemoveDocumentoCancelacionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.UpdateCancelacionVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    /// <summary>
    /// Cancelaciones de documentos de venta (cobranza). Clave: /{code} (10 dígitos).
    /// Flujo: crear (pago + documentos) -> agregar / retirar documentos -> cuadrar -> generar asiento.
    /// Los documentos pendientes se consultan en /api/facturacion/transacciones/cobranzas/documentos-pendientes.
    /// </summary>
    public static class CancelacionesVentaEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/cancelaciones-venta";

        public record AddDocumentosBody(List<CancelacionVentaDocumentoInput> Documents, byte[] RowVersion);

        public record CancelacionRowVersionBody(byte[] RowVersion);

        public record AnularAsientoBody(string Reason, byte[] RowVersion);

        public static void MapCancelacionesVentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Cancelaciones de Venta (Facturación)")
                .RequireAuthorization();

            group.MapGet("/paged", async ([AsParameters] GetCancelacionesVentaPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetCancelacionesVentaPagedQueryString")
            .RequireAuthorization("CANCELACIONESVENTA.VIEW");

            group.MapPost("/paged/body", async (GetCancelacionesVentaPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetCancelacionesVentaPagedBody")
            .RequireAuthorization("CANCELACIONESVENTA.VIEW");

            group.MapGet("/{code}", async (string code, ISender sender) =>
                Results.Ok(await sender.Send(new GetCancelacionVentaByCodeQuery(code))))
            .WithName("GetCancelacionVentaByCode")
            .RequireAuthorization("CANCELACIONESVENTA.VIEW");

            group.MapPost("/", async (CreateCancelacionVentaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.Code}", result);
            })
            .WithName("CreateCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.CREATE");

            group.MapPut("/{code}", async (string code, UpdateCancelacionVentaCommand command, ISender sender) =>
            {
                command.Code = code;
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.UPDATE");

            group.MapDelete("/{code}", async (string code, ISender sender) =>
            {
                await sender.Send(new DeleteCancelacionVentaCommand(code));
                return Results.NoContent();
            })
            .WithName("DeleteCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.DELETE");

            group.MapPost("/{code}/documentos", async (string code, AddDocumentosBody body, ISender sender) =>
                Results.Ok(await sender.Send(new AddDocumentosCancelacionVentaCommand
                {
                    Code = code,
                    Documents = body.Documents,
                    RowVersion = body.RowVersion
                })))
            .WithName("AddDocumentosCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.UPDATE");

            group.MapPost("/{code}/documentos/{item}/retirar", async (
                string code, string item, CancelacionRowVersionBody body, ISender sender) =>
                Results.Ok(await sender.Send(new RemoveDocumentoCancelacionVentaCommand(code, item, body.RowVersion))))
            .WithName("RemoveDocumentoCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.UPDATE");

            group.MapPost("/{code}/documentos/{item}/cuadrar", async (
                string code, string item, CancelacionRowVersionBody body, ISender sender) =>
                Results.Ok(await sender.Send(new CuadrarCancelacionVentaCommand(code, item, body.RowVersion))))
            .WithName("CuadrarCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.UPDATE");

            group.MapPost("/{code}/generar-asiento", async (string code, CancelacionRowVersionBody body, ISender sender) =>
                Results.Ok(await sender.Send(new GenerarAsientoCancelacionVentaCommand(code, body.RowVersion))))
            .WithName("GenerarAsientoCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.GENERAR");

            group.MapPost("/{code}/anular-asiento", async (string code, AnularAsientoBody body, ISender sender) =>
                Results.Ok(await sender.Send(new AnularAsientoCancelacionVentaCommand(code, body.Reason, body.RowVersion))))
            .WithName("AnularAsientoCancelacionVenta")
            .RequireAuthorization("CANCELACIONESVENTA.ANULAR");
        }
    }
}
