using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.CreateEstadoCivil;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.GetEstadoCivilByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.GetEstadosCivilesPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.ToggleEstadoCivilStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.UpdateEstadoCivil;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class EstadosCivilesEndpoints
    {
        public static void MapEstadosCivilesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/estados-civiles")
                .WithTags("EstadosCiviles (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateEstadoCivilCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/estados-civiles/{result.Code}", result);
            })
            .WithName("CreateEstadoCivil")
            .RequireAuthorization("ESTADOSCIVILES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetEstadosCivilesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEstadosCivilesPagedQueryString")
            .RequireAuthorization("ESTADOSCIVILES.VIEW");

            group.MapPost("/paged/body", async (
                GetEstadosCivilesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEstadosCivilesPagedBody")
            .RequireAuthorization("ESTADOSCIVILES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetEstadoCivilByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetEstadoCivilByCode")
            .RequireAuthorization("ESTADOSCIVILES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateEstadoCivilCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateEstadoCivil")
            .RequireAuthorization("ESTADOSCIVILES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleEstadoCivilStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleEstadoCivilStatus")
            .RequireAuthorization("ESTADOSCIVILES.UPDATE");
        }
    }
}