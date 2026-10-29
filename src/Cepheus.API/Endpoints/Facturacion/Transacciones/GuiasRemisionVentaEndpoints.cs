using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.AnularGuiaRemisionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.CreateGuiaRemisionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.GetGuiaProgramacion;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.GetGuiaRemisionVentaByNumber;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.GetGuiasRemisionVentaPaginated;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.MarcarImpresaGuiaRemisionVenta;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.UpdateGuiaRemisionVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Transacciones;

public static class GuiasRemisionVentaEndpoints
{
    private const string Base = "/api/facturacion/transacciones/guias-remision-venta";

    public static void MapGuiasRemisionVentaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Base)
            .WithTags("Guías de Remisión de Ventas (Facturación)")
            .RequireAuthorization();

        group.MapPost("/", async (CreateGuiaRemisionVentaCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"{Base}/{result.Number}", result);
        })
        .WithName("CreateGuiaRemisionVenta")
        .RequireAuthorization("GUIASREMISIONVENTA.CREATE");

        group.MapGet("/paged", async ([AsParameters] GetGuiasRemisionVentaPaginatedQuery query, ISender sender) =>
            Results.Ok(await sender.Send(query)))
        .WithName("GetGuiasRemisionVentaPaginated")
        .RequireAuthorization("GUIASREMISIONVENTA.VIEW");

        group.MapGet("/{number}", async (string number, ISender sender) =>
            Results.Ok(await sender.Send(new GetGuiaRemisionVentaByNumberQuery(number))))
        .WithName("GetGuiaRemisionVentaByNumber")
        .RequireAuthorization("GUIASREMISIONVENTA.VIEW");

        group.MapGet("/programaciones/{type}/{code}", async (string type, string code, ISender sender) =>
            Results.Ok(await sender.Send(new GetGuiaProgramacionQuery(type, code))))
        .WithName("GetGuiaProgramacion")
        .RequireAuthorization("GUIASREMISIONVENTA.VIEW");

        group.MapPut("/{number}", async (string number, UpdateGuiaRemisionVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new UpdateGuiaRemisionVentaCommand(number, body))))
        .WithName("UpdateGuiaRemisionVenta")
        .RequireAuthorization("GUIASREMISIONVENTA.UPDATE");

        group.MapPost("/{number}/anular", async (string number, AnularGuiaRemisionVentaRequest body, ISender sender) =>
            Results.Ok(await sender.Send(new AnularGuiaRemisionVentaCommand(number, body))))
        .WithName("AnularGuiaRemisionVenta")
        .RequireAuthorization("GUIASREMISIONVENTA.ANNUL");

        group.MapPost("/{number}/marcar-impresa", async (string number, ISender sender) =>
            Results.Ok(await sender.Send(new MarcarImpresaGuiaRemisionVentaCommand(number))))
        .WithName("MarcarImpresaGuiaRemisionVenta")
        .RequireAuthorization("GUIASREMISIONVENTA.PRINT");
    }
}
