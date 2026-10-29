using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.CreateTipoHojaCorte;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.GetTipoHojaCorteByCode;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.GetTiposHojaCortePaginated;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.ToggleTipoHojaCorteStatus;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.UpdateTipoHojaCorte;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Catalogos
{
    public static class TiposHojaCorteEndpoints
    {
        public static void MapTiposHojaCorteEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/catalogos/tipos-hoja-corte")
                .WithTags("Tipos de Hoja de Corte (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoHojaCorteCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/facturacion/catalogos/tipos-hoja-corte/{result.Code}", result);
            })
            .WithName("CreateTipoHojaCorte")
            .RequireAuthorization("TIPOSHOJACORTE.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposHojaCortePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposHojaCortePagedQueryString")
            .RequireAuthorization("TIPOSHOJACORTE.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposHojaCortePaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposHojaCortePagedBody")
            .RequireAuthorization("TIPOSHOJACORTE.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoHojaCorteByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoHojaCorteByCode")
            .RequireAuthorization("TIPOSHOJACORTE.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoHojaCorteCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoHojaCorte")
            .RequireAuthorization("TIPOSHOJACORTE.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoHojaCorteStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoHojaCorteStatus")
            .RequireAuthorization("TIPOSHOJACORTE.UPDATE");
        }
    }
}
