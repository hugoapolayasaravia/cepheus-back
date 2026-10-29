using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.CreateTipoSctr;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.GetTipoSctrByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.GetTiposSctrPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.ToggleTipoSctrStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.UpdateTipoSctr;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposSctrEndpoints
    {
        public static void MapTiposSctrEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-sctr")
                .WithTags("TiposSctr (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoSctrCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-sctr/{result.Code}", result);
            })
            .WithName("CreateTipoSctr")
            .RequireAuthorization("TIPOSSCTR.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposSctrPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposSctrPagedQueryString")
            .RequireAuthorization("TIPOSSCTR.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposSctrPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposSctrPagedBody")
            .RequireAuthorization("TIPOSSCTR.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoSctrByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoSctrByCode")
            .RequireAuthorization("TIPOSSCTR.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoSctrCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoSctr")
            .RequireAuthorization("TIPOSSCTR.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoSctrStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoSctrStatus")
            .RequireAuthorization("TIPOSSCTR.UPDATE");
        }
    }
}