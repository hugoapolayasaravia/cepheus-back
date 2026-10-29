using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.CreateTipoAfiliacion;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.GetTipoAfiliacionByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.GetTiposAfiliacionPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.ToggleTipoAfiliacionStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.UpdateTipoAfiliacion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposAfiliacionEndpoints
    {
        public static void MapTiposAfiliacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-afiliacion")
                .WithTags("TiposAfiliacion (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoAfiliacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-afiliacion/{result.Code}", result);
            })
            .WithName("CreateTipoAfiliacion")
            .RequireAuthorization("TIPOSAFILIACION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposAfiliacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposAfiliacionPagedQueryString")
            .RequireAuthorization("TIPOSAFILIACION.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposAfiliacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposAfiliacionPagedBody")
            .RequireAuthorization("TIPOSAFILIACION.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoAfiliacionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoAfiliacionByCode")
            .RequireAuthorization("TIPOSAFILIACION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoAfiliacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoAfiliacion")
            .RequireAuthorization("TIPOSAFILIACION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoAfiliacionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoAfiliacionStatus")
            .RequireAuthorization("TIPOSAFILIACION.UPDATE");
        }
    }
}