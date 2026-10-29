using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.AnularOperacionChequeVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GenerarChequeVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GetChequesVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GetChequeVentaByKey;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.RebotarChequeVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    /// <summary>
    /// Cheques de clientes. No hay POST de alta: el cheque nace en el módulo de Cancelaciones.
    /// Clave: /{cliente}/{banco}/{numero}.
    /// </summary>
    public static class ChequesVentaEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/cheques-venta";

        public record RebotarChequeBody(string MotivoReboteCode, DateTime ReboteDate, byte[] RowVersion);
        public record GenerarChequeBody(string OperationNumber, DateTime OperationDate, byte[] RowVersion);
        public record ChequeRowVersionBody(byte[] RowVersion);

        public static void MapChequesVentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Cheques de Venta (Facturación)")
                .RequireAuthorization();

            group.MapGet("/paged", async ([AsParameters] GetChequesVentaPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetChequesVentaPagedQueryString")
            .RequireAuthorization("CHEQUESVENTA.VIEW");

            group.MapPost("/paged/body", async (GetChequesVentaPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetChequesVentaPagedBody")
            .RequireAuthorization("CHEQUESVENTA.VIEW");

            group.MapGet("/{cliente}/{banco}/{numero}", async (string cliente, string banco, string numero, ISender sender) =>
                Results.Ok(await sender.Send(new GetChequeVentaByKeyQuery(cliente, banco, numero))))
            .WithName("GetChequeVentaByKey")
            .RequireAuthorization("CHEQUESVENTA.VIEW");

            group.MapPost("/{cliente}/{banco}/{numero}/rebotar", async (
                string cliente, string banco, string numero, RebotarChequeBody body, ISender sender) =>
                Results.Ok(await sender.Send(new RebotarChequeVentaCommand(
                    cliente, banco, numero, body.MotivoReboteCode, body.ReboteDate, body.RowVersion))))
            .WithName("RebotarChequeVenta")
            .RequireAuthorization("CHEQUESVENTA.REBOTAR");

            group.MapPost("/{cliente}/{banco}/{numero}/generar", async (
                string cliente, string banco, string numero, GenerarChequeBody body, ISender sender) =>
                Results.Ok(await sender.Send(new GenerarChequeVentaCommand(
                    cliente, banco, numero, body.OperationNumber, body.OperationDate, body.RowVersion))))
            .WithName("GenerarChequeVenta")
            .RequireAuthorization("CHEQUESVENTA.GENERAR");

            group.MapPost("/{cliente}/{banco}/{numero}/anular-operacion", async (
                string cliente, string banco, string numero, ChequeRowVersionBody body, ISender sender) =>
                Results.Ok(await sender.Send(new AnularOperacionChequeVentaCommand(cliente, banco, numero, body.RowVersion))))
            .WithName("AnularOperacionChequeVenta")
            .RequireAuthorization("CHEQUESVENTA.ANULAR");
        }
    }
}
