using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.CreateAlergia;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.GetAlergiaByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.GetAlergiasPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.ToggleAlergiaStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.UpdateAlergia;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class AlergiasEndpoints
    {
        public static void MapAlergiasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/alergias")
                .WithTags("Alergias (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAlergiaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/alergias/{result.Code}", result);
            })
            .WithName("CreateAlergia")
            .RequireAuthorization("ALERGIAS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAlergiasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAlergiasPagedQueryString")
            .RequireAuthorization("ALERGIAS.VIEW");

            group.MapPost("/paged/body", async (
                GetAlergiasPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAlergiasPagedBody")
            .RequireAuthorization("ALERGIAS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAlergiaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetAlergiaByCode")
            .RequireAuthorization("ALERGIAS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateAlergiaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateAlergia")
            .RequireAuthorization("ALERGIAS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleAlergiaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleAlergiaStatus")
            .RequireAuthorization("ALERGIAS.UPDATE");
        }
    }
}