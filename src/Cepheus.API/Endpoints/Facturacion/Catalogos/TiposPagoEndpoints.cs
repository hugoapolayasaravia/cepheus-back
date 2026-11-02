using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.CreateTipoPago;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.GetTipoPagoByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.GetTiposPagoPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.ToggleTipoPagoStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.UpdateTipoPago;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    /// <summary>Tipos de pago de las cancelaciones, con sus cuentas contables por moneda.</summary>
    public static class TiposPagoEndpoints
    {
        private const string Route = "/api/facturacion/catalogos/tipos-pago";

        public static void MapTiposPagoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(Route)
                .WithTags("Tipos de Pago (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoPagoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"{Route}/{result.Code}", result);
            })
            .WithName("CreateTipoPago")
            .RequireAuthorization("TIPOSPAGO.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetTiposPagoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetTiposPagoPagedQueryString")
            .RequireAuthorization("TIPOSPAGO.VIEW");

            group.MapPost("/paged/body", async (GetTiposPagoPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetTiposPagoPagedBody")
            .RequireAuthorization("TIPOSPAGO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
                Results.Ok(await sender.Send(new GetTipoPagoByCodeQuery(codigo))))
            .WithName("GetTipoPagoByCode")
            .RequireAuthorization("TIPOSPAGO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoPagoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateTipoPago")
            .RequireAuthorization("TIPOSPAGO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
                Results.Ok(new { IsActive = await sender.Send(new ToggleTipoPagoStatusCommand(codigo)) }))
            .WithName("ToggleTipoPagoStatus")
            .RequireAuthorization("TIPOSPAGO.UPDATE");
        }
    }
}
