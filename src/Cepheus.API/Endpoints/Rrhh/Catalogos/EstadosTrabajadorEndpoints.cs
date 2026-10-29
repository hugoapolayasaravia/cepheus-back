using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.CreateEstadoTrabajador;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.GetEstadoTrabajadorByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.GetEstadosTrabajadorPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.ToggleEstadoTrabajadorStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.UpdateEstadoTrabajador;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class EstadosTrabajadorEndpoints
    {
        public static void MapEstadosTrabajadorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/estados-trabajador")
                .WithTags("EstadosTrabajador (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateEstadoTrabajadorCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/estados-trabajador/{result.Code}", result);
            })
            .WithName("CreateEstadoTrabajador")
            .RequireAuthorization("ESTADOSTRABAJADOR.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetEstadosTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEstadosTrabajadorPagedQueryString")
            .RequireAuthorization("ESTADOSTRABAJADOR.VIEW");

            group.MapPost("/paged/body", async (
                GetEstadosTrabajadorPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEstadosTrabajadorPagedBody")
            .RequireAuthorization("ESTADOSTRABAJADOR.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetEstadoTrabajadorByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetEstadoTrabajadorByCode")
            .RequireAuthorization("ESTADOSTRABAJADOR.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateEstadoTrabajadorCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateEstadoTrabajador")
            .RequireAuthorization("ESTADOSTRABAJADOR.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleEstadoTrabajadorStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleEstadoTrabajadorStatus")
            .RequireAuthorization("ESTADOSTRABAJADOR.UPDATE");
        }
    }
}