using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.CreateTipoOrden;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.GetTipoOrdenByCode;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.GetTiposOrdenPaginated;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.ToggleTipoOrdenStatus;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.UpdateTipoOrden;
using MediatR;

namespace Cepheus.API.Endpoints.Mantenimiento.Catalogos
{
    public static class TiposOrdenEndpoints
    {
        public static void MapTiposOrdenEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/mantenimiento/catalogos/tipos-orden")
                .WithTags("Tipos de Orden (Mantenimiento)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoOrdenCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/mantenimiento/catalogos/tipos-orden/{result.Code}", result);
            })
            .WithName("CreateTipoOrden")
            .RequireAuthorization("TIPOSORDEN.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposOrdenPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposOrdenPagedQueryString")
            .RequireAuthorization("TIPOSORDEN.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposOrdenPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposOrdenPagedBody")
            .RequireAuthorization("TIPOSORDEN.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoOrdenByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoOrdenByCode")
            .RequireAuthorization("TIPOSORDEN.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoOrdenCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoOrden")
            .RequireAuthorization("TIPOSORDEN.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoOrdenStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoOrdenStatus")
            .RequireAuthorization("TIPOSORDEN.UPDATE");
        }
    }
}
