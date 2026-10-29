using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.AnularNotaDebitoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.CreateNotaDebitoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.EmitirNotaDebitoVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.GetNotaDebitoVentaByKey;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.GetNotaDebitoVentaReport;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.GetNotasDebitoVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.UpdateNotaDebitoVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones
{
    public static class NotasDebitoVentaEndpoints
    {
        private const string Route = "/api/facturacion/transacciones/notas-debito-venta";

        public static void MapNotasDebitoVentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Notas de Débito de Venta (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateNotaDebitoVentaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.PlantaCode}/{result.Number}", result);
            })
            .WithName("CreateNotaDebitoVenta")
            .RequireAuthorization("NOTASDEBITOVENTA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetNotasDebitoVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNotasDebitoVentaPagedQueryString")
            .RequireAuthorization("NOTASDEBITOVENTA.VIEW");

            group.MapPost("/paged/body", async (
                GetNotasDebitoVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetNotasDebitoVentaPagedBody")
            .RequireAuthorization("NOTASDEBITOVENTA.VIEW");

            group.MapGet("/{planta}/{numero}", async (string planta, string numero, ISender sender) =>
            {
                var result = await sender.Send(new GetNotaDebitoVentaByKeyQuery(planta, numero));
                return Results.Ok(result);
            })
            .WithName("GetNotaDebitoVentaByKey")
            .RequireAuthorization("NOTASDEBITOVENTA.VIEW");

            group.MapGet("/{planta}/{numero}/reporte", async (string planta, string numero, ISender sender) =>
            {
                var result = await sender.Send(new GetNotaDebitoVentaReportQuery(planta, numero));
                return Results.Ok(result);
            })
            .WithName("GetNotaDebitoVentaReport")
            .RequireAuthorization("NOTASDEBITOVENTA.VIEW");

            group.MapPut("/{planta}/{numero}", async (
                string planta, string numero, UpdateNotaDebitoVentaCommand command, ISender sender) =>
            {
                if (!string.Equals(planta, command.PlantaCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(numero, command.Number, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con el cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateNotaDebitoVenta")
            .RequireAuthorization("NOTASDEBITOVENTA.UPDATE");

            group.MapPatch("/{planta}/{numero}/emitir", async (string planta, string numero, ISender sender) =>
            {
                var result = await sender.Send(new EmitirNotaDebitoVentaCommand(planta, numero));
                return Results.Ok(result);
            })
            .WithName("EmitirNotaDebitoVenta")
            .RequireAuthorization("NOTASDEBITOVENTA.UPDATE");

            group.MapPatch("/{planta}/{numero}/anular", async (
                string planta, string numero, AnularNotaDebitoVentaRequestBody body, ISender sender) =>
            {
                var result = await sender.Send(new AnularNotaDebitoVentaCommand(planta, numero, body.Reason));
                return Results.Ok(result);
            })
            .WithName("AnularNotaDebitoVenta")
            .RequireAuthorization("NOTASDEBITOVENTA.ANULAR");
        }
    }

    public record AnularNotaDebitoVentaRequestBody(string Reason);
}
