using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.CreateSexo;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.GetSexoByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.GetSexosPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.ToggleSexoStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.UpdateSexo;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class SexosEndpoints
    {
        public static void MapSexosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/sexos")
                .WithTags("Sexos (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateSexoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/sexos/{result.Code}", result);
            })
            .WithName("CreateSexo")
            .RequireAuthorization("SEXOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetSexosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSexosPagedQueryString")
            .RequireAuthorization("SEXOS.VIEW");

            group.MapPost("/paged/body", async (
                GetSexosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSexosPagedBody")
            .RequireAuthorization("SEXOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetSexoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetSexoByCode")
            .RequireAuthorization("SEXOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateSexoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateSexo")
            .RequireAuthorization("SEXOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleSexoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleSexoStatus")
            .RequireAuthorization("SEXOS.UPDATE");
        }
    }
}