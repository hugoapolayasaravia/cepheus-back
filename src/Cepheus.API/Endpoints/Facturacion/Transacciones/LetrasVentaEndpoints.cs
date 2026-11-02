using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.EliminarDocumentoLetraVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GenerarLetrasVarias;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaReport;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetrasVentaReportLote;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetrasVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaByKey;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.ProtestarLetraVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.UpdateLetraVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    /// <summary>
    /// Letras de clientes. Nacen en Cancelaciones (tipo de pago 03) o aquí con "Letras Varias" (tipo 11).
    /// Clave: /{planta}/{numero}.
    /// </summary>
    public static class LetrasVentaEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/letras-venta";

        public record UpdateLetraBody(
            DateTime IssueDate, DateTime DueDate, bool AcceptedByClient, string? Description,
            bool ExchangeDocuments, byte[] RowVersion);

        public record LetraRowVersionBody(byte[] RowVersion);

        public static void MapLetrasVentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Letras de Venta (Facturación)")
                .RequireAuthorization();

            group.MapGet("/paged", async ([AsParameters] GetLetrasVentaPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetLetrasVentaPagedQueryString")
            .RequireAuthorization("LETRASVENTA.VIEW");

            group.MapPost("/paged/body", async (GetLetrasVentaPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetLetrasVentaPagedBody")
            .RequireAuthorization("LETRASVENTA.VIEW");

            group.MapGet("/{planta}/{numero}", async (string planta, string numero, ISender sender) =>
                Results.Ok(await sender.Send(new GetLetraVentaByKeyQuery(planta, numero))))
            .WithName("GetLetraVentaByKey")
            .RequireAuthorization("LETRASVENTA.VIEW");

            // Impresión: ?aval=<secuencia del responsable (maestro Clientes responsables de letra)>&opcion=S|N (legacy: letra + opción + aval).
            group.MapGet("/{planta}/{numero}/reporte", async (
                string planta, string numero, string? aval, string? opcion, ISender sender) =>
                Results.Ok(await sender.Send(new GetLetraVentaReportQuery(planta, numero, aval, opcion ?? "S"))))
            .WithName("GetLetraVentaReport")
            .RequireAuthorization("LETRASVENTA.VIEW");

            // Impresión por lote (legacy w_cre_rpt_x_lote_letras).
            group.MapPost("/reporte-lote", async (GetLetrasVentaReportLoteQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetLetrasVentaReportLote")
            .RequireAuthorization("LETRASVENTA.VIEW");

            group.MapPut("/{planta}/{numero}", async (
                string planta, string numero, UpdateLetraBody body, ISender sender) =>
                Results.Ok(await sender.Send(new UpdateLetraVentaCommand(
                    planta, numero, body.IssueDate, body.DueDate, body.AcceptedByClient, body.Description,
                    body.ExchangeDocuments, body.RowVersion))))
            .WithName("UpdateLetraVenta")
            .RequireAuthorization("LETRASVENTA.UPDATE");

            group.MapPost("/{planta}/{numero}/protestar", async (
                string planta, string numero, LetraRowVersionBody body, ISender sender) =>
                Results.Ok(await sender.Send(new ProtestarLetraVentaCommand(planta, numero, body.RowVersion))))
            .WithName("ProtestarLetraVenta")
            .RequireAuthorization("LETRASVENTA.PROTESTAR");

            group.MapDelete("/{planta}/{numero}/documentos/{item}", async (
                string planta, string numero, string item, ISender sender) =>
                Results.Ok(await sender.Send(new EliminarDocumentoLetraVentaCommand(planta, numero, item))))
            .WithName("EliminarDocumentoLetraVenta")
            .RequireAuthorization("LETRASVENTA.UPDATE");

            // Letras Varias: vista previa de cuotas (no guarda) y generación.
            group.MapPost("/letras-varias/preview", async (GenerarLetrasVariasCommand command, ISender sender) =>
                Results.Ok(await sender.Send(command with { PreviewOnly = true })))
            .WithName("PreviewLetrasVarias")
            .RequireAuthorization("LETRASVENTA.CREATE");

            group.MapPost("/letras-varias", async (GenerarLetrasVariasCommand command, ISender sender) =>
            {
                var result = await sender.Send(command with { PreviewOnly = false });
                return Results.Created($"{Route}/paged?CancelacionCode={result.CancelacionCode}", result);
            })
            .WithName("GenerarLetrasVarias")
            .RequireAuthorization("LETRASVENTA.CREATE");
        }
    }
}
