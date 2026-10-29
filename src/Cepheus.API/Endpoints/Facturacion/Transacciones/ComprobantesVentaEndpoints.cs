using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.AgregarGuiasComprobanteVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.AnularComprobanteVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVentaLibre;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.GetComprobanteVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.GetComprobantesVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.GetGuiasPendientesFacturar;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.MarcarImpresaComprobanteVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.QuitarGuiaComprobanteVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.TransferenciaGratuitaComprobanteVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.UpdateComprobanteVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones;

public static class ComprobantesVentaEndpoints
{
    private const string Base = "/api/facturacion/transacciones/comprobantes-venta";

    public static void MapComprobantesVentaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Base)
            .WithTags("Comprobantes de Pago de Ventas (Facturación)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateComprobanteVentaCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"{Base}/{result.TypeCode}/{result.Number}", result);
        })
        .WithName("CreateComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.CREATE");

        group.MapPost("/libre", async (CreateComprobanteVentaLibreCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"{Base}/{result.TypeCode}/{result.Number}", result);
        })
        .WithName("CreateComprobanteVentaLibre")
        .RequireAuthorization("COMPROBANTESVENTA.CREATE");

        group.MapGet("/paged", async ([AsParameters] GetComprobantesVentaPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetComprobantesVentaPaginated")
        .RequireAuthorization("COMPROBANTESVENTA.VIEW");

        group.MapGet("/guias-pendientes", async ([AsParameters] GetGuiasPendientesFacturarQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetGuiasPendientesFacturar")
        .RequireAuthorization("COMPROBANTESVENTA.VIEW");

        group.MapGet("/{type}/{number}", async (string type, string number, ISender sender) =>
            Results.Ok(await sender.Send(new GetComprobanteVentaQuery(type, number))))
        .WithName("GetComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.VIEW");

        group.MapPut("/{type}/{number}", async (string type, string number, UpdateComprobanteVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateComprobanteVentaCommand(type, number, body))))
        .WithName("UpdateComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.UPDATE");

        group.MapPost("/{type}/{number}/guias", async (string type, string number, AgregarGuiasComprobanteVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new AgregarGuiasComprobanteVentaCommand(type, number, body))))
        .WithName("AgregarGuiasComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.UPDATE");

        group.MapDelete("/{type}/{number}/guias/{guia}", async (string type, string number, string guia, ISender sender) =>
            Results.Ok(await sender.Send(new QuitarGuiaComprobanteVentaCommand(type, number, guia))))
        .WithName("QuitarGuiaComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.UPDATE");

        group.MapPost("/{type}/{number}/transferencia-gratuita", async (string type, string number, TransferenciaGratuitaComprobanteVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new TransferenciaGratuitaComprobanteVentaCommand(type, number, body))))
        .WithName("TransferenciaGratuitaComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.UPDATE");

        group.MapPost("/{type}/{number}/anular", async (string type, string number, AnularComprobanteVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new AnularComprobanteVentaCommand(type, number, body))))
        .WithName("AnularComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.ANNUL");

        group.MapPost("/{type}/{number}/marcar-impresa", async (string type, string number, ISender sender) =>
            Results.Ok(await sender.Send(new MarcarImpresaComprobanteVentaCommand(type, number))))
        .WithName("MarcarImpresaComprobanteVenta")
        .RequireAuthorization("COMPROBANTESVENTA.PRINT");
    }
}
