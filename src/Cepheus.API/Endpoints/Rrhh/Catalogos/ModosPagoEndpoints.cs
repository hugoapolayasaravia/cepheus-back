using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.CreateModoPago;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.GetModoPagoByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.GetModosPagoPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.ToggleModoPagoStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.UpdateModoPago;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class ModosPagoEndpoints
    {
        public static void MapModosPagoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/modos-pago")
                .WithTags("ModosPago (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateModoPagoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/modos-pago/{result.Code}", result);
            })
            .WithName("CreateModoPago")
            .RequireAuthorization("MODOSPAGO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetModosPagoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetModosPagoPagedQueryString")
            .RequireAuthorization("MODOSPAGO.VIEW");

            group.MapPost("/paged/body", async (
                GetModosPagoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetModosPagoPagedBody")
            .RequireAuthorization("MODOSPAGO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetModoPagoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetModoPagoByCode")
            .RequireAuthorization("MODOSPAGO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateModoPagoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateModoPago")
            .RequireAuthorization("MODOSPAGO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleModoPagoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleModoPagoStatus")
            .RequireAuthorization("MODOSPAGO.UPDATE");
        }
    }
}