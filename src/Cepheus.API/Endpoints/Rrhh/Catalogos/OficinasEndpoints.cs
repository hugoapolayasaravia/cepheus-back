using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.CreateOficina;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.GetOficinaByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.GetOficinasPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.ToggleOficinaStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.UpdateOficina;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class OficinasEndpoints
    {
        public static void MapOficinasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/oficinas")
                .WithTags("Oficinas (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateOficinaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/oficinas/{result.Code}", result);
            })
            .WithName("CreateOficina")
            .RequireAuthorization("OFICINAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetOficinasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOficinasPagedQueryString")
            .RequireAuthorization("OFICINAS.VIEW");

            group.MapPost("/paged/body", async (
                GetOficinasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetOficinasPagedBody")
            .RequireAuthorization("OFICINAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetOficinaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetOficinaByCode")
            .RequireAuthorization("OFICINAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateOficinaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateOficina")
            .RequireAuthorization("OFICINAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleOficinaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleOficinaStatus")
            .RequireAuthorization("OFICINAS.UPDATE");
        }
    }
}