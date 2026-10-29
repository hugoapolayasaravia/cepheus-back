using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.CreateTipoVia;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.GetTipoViaByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.GetTiposViaPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.ToggleTipoViaStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.UpdateTipoVia;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposViaEndpoints
    {
        public static void MapTiposViaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-via")
                .WithTags("TiposVia (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoViaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-via/{result.Code}", result);
            })
            .WithName("CreateTipoVia")
            .RequireAuthorization("TIPOSVIA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposViaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposViaPagedQueryString")
            .RequireAuthorization("TIPOSVIA.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposViaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposViaPagedBody")
            .RequireAuthorization("TIPOSVIA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoViaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoViaByCode")
            .RequireAuthorization("TIPOSVIA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoViaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoVia")
            .RequireAuthorization("TIPOSVIA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoViaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoViaStatus")
            .RequireAuthorization("TIPOSVIA.UPDATE");
        }
    }
}