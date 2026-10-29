using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.AnularNotaCreditoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.CreateNotaCreditoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.EmitirNotaCreditoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GenerateNotaCreditoVentaFromSolicitud;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GetNotaCreditoVentaByKey;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GetNotasCreditoVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.UpdateNotaCreditoVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class NotasCreditoVentaEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/notas-credito-venta";

        public static void MapNotasCreditoVentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Notas de Crédito de Venta (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNotaCreditoVentaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.PlantaCode}/{result.Number}", result);
            })
            .WithName("CreateNotaCreditoVenta")
            .RequireAuthorization("NOTASCREDITOVENTA.CREATE");

            group.MapPost("/desde-solicitud", async (
                GenerateNotaCreditoVentaFromSolicitudCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.PlantaCode}/{result.Number}", result);
            })
            .WithName("GenerateNotaCreditoVentaFromSolicitud")
            .RequireAuthorization("NOTASCREDITOVENTA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNotasCreditoVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNotasCreditoVentaPagedQueryString")
            .RequireAuthorization("NOTASCREDITOVENTA.VIEW");

            group.MapPost("/paged/body", async (
                GetNotasCreditoVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNotasCreditoVentaPagedBody")
            .RequireAuthorization("NOTASCREDITOVENTA.VIEW");

            group.MapGet("/{planta}/{numero}", async (string planta, string numero, ISender sender) =>
            {
                var result = await sender.Send(new GetNotaCreditoVentaByKeyQuery(planta, numero));
                return Results.Ok(result);
            })
            .WithName("GetNotaCreditoVentaByKey")
            .RequireAuthorization("NOTASCREDITOVENTA.VIEW");

            group.MapPut("/{planta}/{numero}", async (
                string planta, string numero, UpdateNotaCreditoVentaCommand command, ISender sender) =>
            {
                if (!string.Equals(planta, command.PlantaCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(numero, command.Number, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNotaCreditoVenta")
            .RequireAuthorization("NOTASCREDITOVENTA.UPDATE");

            group.MapPatch("/{planta}/{numero}/emitir", async (string planta, string numero, ISender sender) =>
            {
                var result = await sender.Send(new EmitirNotaCreditoVentaCommand(planta, numero));
                return Results.Ok(result);
            })
            .WithName("EmitirNotaCreditoVenta")
            .RequireAuthorization("NOTASCREDITOVENTA.UPDATE");

            group.MapPatch("/{planta}/{numero}/anular", async (
                string planta, string numero, AnularNotaCreditoVentaRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new AnularNotaCreditoVentaCommand(planta, numero, body.Reason));
                return Results.Ok(result);
            })
            .WithName("AnularNotaCreditoVenta")
            .RequireAuthorization("NOTASCREDITOVENTA.ANULAR");
        }
    }

    public record AnularNotaCreditoVentaRequestBody(string Reason);
}
