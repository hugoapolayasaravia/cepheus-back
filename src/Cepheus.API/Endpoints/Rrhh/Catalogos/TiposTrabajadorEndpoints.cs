using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.CreateTipoTrabajador;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.GetTipoTrabajadorByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.GetTiposTrabajadorPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.ToggleTipoTrabajadorStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.UpdateTipoTrabajador;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposTrabajadorEndpoints
    {
        public static void MapTiposTrabajadorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-trabajador")
                .WithTags("TiposTrabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoTrabajadorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-trabajador/{result.Code}", result);
            })
            .WithName("CreateTipoTrabajador")
            .RequireAuthorization("TIPOSTRABAJADOR.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposTrabajadorPagedQueryString")
            .RequireAuthorization("TIPOSTRABAJADOR.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposTrabajadorPagedBody")
            .RequireAuthorization("TIPOSTRABAJADOR.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoTrabajadorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoTrabajadorByCode")
            .RequireAuthorization("TIPOSTRABAJADOR.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoTrabajadorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoTrabajador")
            .RequireAuthorization("TIPOSTRABAJADOR.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoTrabajadorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoTrabajadorStatus")
            .RequireAuthorization("TIPOSTRABAJADOR.UPDATE");
        }
    }
}