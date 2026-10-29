using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.CreateTipoCentroFormacion;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.GetTipoCentroFormacionByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.GetTiposCentroFormacionPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.ToggleTipoCentroFormacionStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.UpdateTipoCentroFormacion;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposCentroFormacionEndpoints
    {
        public static void MapTiposCentroFormacionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-centro-formacion")
                .WithTags("TiposCentroFormacion (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoCentroFormacionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-centro-formacion/{result.Code}", result);
            })
            .WithName("CreateTipoCentroFormacion")
            .RequireAuthorization("TIPOSCENTROFORMACION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposCentroFormacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCentroFormacionPagedQueryString")
            .RequireAuthorization("TIPOSCENTROFORMACION.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposCentroFormacionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCentroFormacionPagedBody")
            .RequireAuthorization("TIPOSCENTROFORMACION.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoCentroFormacionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoCentroFormacionByCode")
            .RequireAuthorization("TIPOSCENTROFORMACION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoCentroFormacionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoCentroFormacion")
            .RequireAuthorization("TIPOSCENTROFORMACION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoCentroFormacionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoCentroFormacionStatus")
            .RequireAuthorization("TIPOSCENTROFORMACION.UPDATE");
        }
    }
}