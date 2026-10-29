using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.CreateTipoValorizacion;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.GetTipoValorizacionByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.GetTiposValorizacionPaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.ToggleTipoValorizacionStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.UpdateTipoValorizacion;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class TiposValorizacionEndpoints
    {
        public static void MapTiposValorizacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/tipos-valorizacion")
                .WithTags("Tipos de Valorización (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoValorizacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/tipos-valorizacion/{result.Code}", result);
            })
            .WithName("CreateTipoValorizacion")
            .RequireAuthorization("TIPOSVALORIZACION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposValorizacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposValorizacionPagedQueryString")
            .RequireAuthorization("TIPOSVALORIZACION.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposValorizacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposValorizacionPagedBody")
            .RequireAuthorization("TIPOSVALORIZACION.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoValorizacionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoValorizacionByCode")
            .RequireAuthorization("TIPOSVALORIZACION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoValorizacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoValorizacion")
            .RequireAuthorization("TIPOSVALORIZACION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoValorizacionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoValorizacionStatus")
            .RequireAuthorization("TIPOSVALORIZACION.UPDATE");
        }
    }
}
