using System.Text;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CancelarDetraccion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CreateDetraccion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.DeleteDetraccion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.DeleteDetraccionItem;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GenerarArchivoDetraccion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionByKey;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionesPaginated;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionItemDocumentos;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetReporteDetraccion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.ImportarConstanciasDetraccion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.SetDetraccionDocumentoExportacion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccion;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccionDocumentoConstancia;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Transacciones
{
    public static class DetraccionesEndpoints
    {
        private const string Route = "/api/contabilidad/transacciones/detracciones";

        public static void MapDetraccionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Detracciones de Adquiriente (Contabilidad)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateDetraccionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.Year}/{result.Period}/{result.LotNumber}", result);
            })
            .WithName("CreateDetraccion")
            .RequireAuthorization("DETRACCIONES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetDetraccionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetDetraccionesPagedQueryString")
            .RequireAuthorization("DETRACCIONES.VIEW");

            group.MapPost("/paged/body", async (
                GetDetraccionesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetDetraccionesPagedBody")
            .RequireAuthorization("DETRACCIONES.VIEW");

            group.MapGet("/{year}/{period}/{lot}", async (string year, string period, string lot, ISender sender) =>
            {
                var result = await sender.Send(new GetDetraccionByKeyQuery(year, period, lot));
                return Results.Ok(result);
            })
            .WithName("GetDetraccionByKey")
            .RequireAuthorization("DETRACCIONES.VIEW");

            group.MapPut("/{year}/{period}/{lot}", async (
                string year, string period, string lot, UpdateDetraccionRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new UpdateDetraccionCommand(year, period, lot, body.StartDate, body.EndDate));
                return Results.Ok(result);
            })
            .WithName("UpdateDetraccion")
            .RequireAuthorization("DETRACCIONES.UPDATE");

            group.MapDelete("/{year}/{period}/{lot}", async (string year, string period, string lot, ISender sender) =>
            {
                await sender.Send(new DeleteDetraccionCommand(year, period, lot));
                return Results.NoContent();
            })
            .WithName("DeleteDetraccion")
            .RequireAuthorization("DETRACCIONES.DELETE");

            group.MapPost("/{year}/{period}/{lot}/cancelar", async (
                string year, string period, string lot, CancelarDetraccionRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new CancelarDetraccionCommand(
                    year, period, lot, body.CancellationDate, body.PaymentReceiptNumber));
                return Results.Ok(result);
            })
            .WithName("CancelarDetraccion")
            .RequireAuthorization("DETRACCIONES.CANCELAR");

            group.MapPost("/{year}/{period}/{lot}/archivo-deposito", async (
                string year, string period, string lot, ISender sender) =>
            {
                var file = await sender.Send(new GenerarArchivoDetraccionCommand(year, period, lot));
                return Results.File(file.Content, file.ContentType, file.FileName);
            })
            .WithName("GenerarArchivoDetraccion")
            .RequireAuthorization("DETRACCIONES.GENERAR");

            group.MapPost("/{year}/{period}/{lot}/constancias/importar", async (
                string year, string period, string lot, IFormFile file, ISender sender) =>
            {
                // El archivo del banco es ANSI: Latin1 conserva todos los bytes.
                using var reader = new StreamReader(file.OpenReadStream(), Encoding.Latin1);
                var content = await reader.ReadToEndAsync();

                var result = await sender.Send(new ImportarConstanciasDetraccionCommand(year, period, lot, content));
                return Results.Ok(result);
            })
            .DisableAntiforgery()
            .WithName("ImportarConstanciasDetraccion")
            .RequireAuthorization("DETRACCIONES.IMPORTAR");

            group.MapGet("/{year}/{period}/{lot}/reporte", async (string year, string period, string lot, ISender sender) =>
            {
                var result = await sender.Send(new GetReporteDetraccionQuery(year, period, lot));
                return Results.Ok(result);
            })
            .WithName("GetReporteDetraccion")
            .RequireAuthorization("DETRACCIONES.VIEW");

            group.MapGet("/{year}/{period}/{lot}/items/{proveedor}/{item:int}/documentos", async (
                string year, string period, string lot, string proveedor, int item, ISender sender) =>
            {
                var result = await sender.Send(new GetDetraccionItemDocumentosQuery(year, period, lot, proveedor, item));
                return Results.Ok(result);
            })
            .WithName("GetDetraccionItemDocumentos")
            .RequireAuthorization("DETRACCIONES.VIEW");

            group.MapDelete("/{year}/{period}/{lot}/items/{proveedor}/{item:int}", async (
                string year, string period, string lot, string proveedor, int item, ISender sender) =>
            {
                var result = await sender.Send(new DeleteDetraccionItemCommand(year, period, lot, proveedor, item));
                return Results.Ok(result);
            })
            .WithName("DeleteDetraccionItem")
            .RequireAuthorization("DETRACCIONES.UPDATE");

            group.MapPatch("/{year}/{period}/{lot}/items/{proveedor}/{item:int}/documentos/{comprobante}/{numero}/exportacion", async (
                string year, string period, string lot, string proveedor, int item,
                string comprobante, string numero, SetExportacionRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new SetDetraccionDocumentoExportacionCommand(
                    year, period, lot, proveedor, item, comprobante, numero, body.IncludeInExport));
                return Results.Ok(result);
            })
            .WithName("SetDetraccionDocumentoExportacion")
            .RequireAuthorization("DETRACCIONES.UPDATE");

            group.MapPut("/{year}/{period}/{lot}/items/{proveedor}/{item:int}/documentos/{comprobante}/{numero}/constancia", async (
                string year, string period, string lot, string proveedor, int item,
                string comprobante, string numero, ConstanciaManualRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new UpdateDetraccionDocumentoConstanciaCommand(
                    year, period, lot, proveedor, item, comprobante, numero,
                    body.OperacionDetraccion, body.ConstanciaDetraccion, body.FechaConstanciaDetraccion));
                return Results.Ok(result);
            })
            .WithName("UpdateDetraccionDocumentoConstancia")
            .RequireAuthorization("DETRACCIONES.UPDATE");
        }
    }

    public record UpdateDetraccionRequestBody(DateTime StartDate, DateTime EndDate);

    public record CancelarDetraccionRequestBody(DateTime CancellationDate, string PaymentReceiptNumber);

    public record SetExportacionRequestBody(bool IncludeInExport);

    public record ConstanciaManualRequestBody(
        string? OperacionDetraccion, string? ConstanciaDetraccion, DateTime? FechaConstanciaDetraccion);
}
