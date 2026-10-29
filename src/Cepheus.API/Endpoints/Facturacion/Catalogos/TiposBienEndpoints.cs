using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.CreateTipoBien;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.GetTipoBienById;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.GetTiposBienPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.ToggleTipoBienStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.UpdateTipoBien;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class TiposBienEndpoints
    {
        public static void MapTiposBienEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/tipos-bien")
                .WithTags("Tipos de bien (detracción)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoBienCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/tipos-bien/{result.Code}", result);
            })
            .WithName("CreateTipoBien")
            .RequireAuthorization("TIPOSBIEN.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposBienPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposBienPagedQueryString")
            .RequireAuthorization("TIPOSBIEN.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposBienPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposBienPagedBody")
            .RequireAuthorization("TIPOSBIEN.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoBienByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoBienById")
            .RequireAuthorization("TIPOSBIEN.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoBienCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoBien")
            .RequireAuthorization("TIPOSBIEN.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoBienStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoBienStatus")
            .RequireAuthorization("TIPOSBIEN.UPDATE");
        }
    }
}
