using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.CreateTipoAsiento;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.GetTipoAsientoById;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.GetTiposAsientoPaginated;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.ToggleTipoAsientoStatus;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.UpdateTipoAsiento;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Catalogos
{
    public static class TiposAsientoEndpoints
    {
        public static void MapTiposAsientoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/catalogos/tipos-asiento")
                .WithTags("Tipos de asiento contable")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoAsientoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/catalogos/tipos-asiento/{result.Code}", result);
            })
            .WithName("CreateTipoAsiento")
            .RequireAuthorization("TIPOSASIENTO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposAsientoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposAsientoPagedQueryString")
            .RequireAuthorization("TIPOSASIENTO.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposAsientoPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposAsientoPagedBody")
            .RequireAuthorization("TIPOSASIENTO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoAsientoByIdQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoAsientoById")
            .RequireAuthorization("TIPOSASIENTO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoAsientoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoAsiento")
            .RequireAuthorization("TIPOSASIENTO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoAsientoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoAsientoStatus")
            .RequireAuthorization("TIPOSASIENTO.UPDATE");
        }
    }
}
