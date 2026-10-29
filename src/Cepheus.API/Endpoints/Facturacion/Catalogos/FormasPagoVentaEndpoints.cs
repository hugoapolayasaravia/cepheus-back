using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.CreateFormaPagoVenta;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.GetFormaPagoVentaByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.GetFormasPagoVentaPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.ToggleFormaPagoVentaStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.UpdateFormaPagoVenta;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class FormasPagoVentaEndpoints
    {
        public static void MapFormasPagoVentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/formas-pago-venta")
                .WithTags("Formas de Pago de Ventas (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateFormaPagoVentaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/formas-pago-venta/{result.Code}", result);
            })
            .WithName("CreateFormaPagoVenta")
            .RequireAuthorization("FORMASPAGOVENTA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetFormasPagoVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFormasPagoVentaPagedQueryString")
            .RequireAuthorization("FORMASPAGOVENTA.VIEW");

            group.MapPost("/paged/body", async (
                GetFormasPagoVentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFormasPagoVentaPagedBody")
            .RequireAuthorization("FORMASPAGOVENTA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetFormaPagoVentaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetFormaPagoVentaByCode")
            .RequireAuthorization("FORMASPAGOVENTA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateFormaPagoVentaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFormaPagoVenta")
            .RequireAuthorization("FORMASPAGOVENTA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleFormaPagoVentaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFormaPagoVentaStatus")
            .RequireAuthorization("FORMASPAGOVENTA.UPDATE");
        }
    }
}
