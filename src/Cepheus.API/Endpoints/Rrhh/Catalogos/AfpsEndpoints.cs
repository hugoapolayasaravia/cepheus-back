using Cepheus.Application.Features.Rrhh.Catalogos.Afps.CreateAfp;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.GetAfpByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.GetAfpsPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.ToggleAfpStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.UpdateAfp;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class AfpsEndpoints
    {
        public static void MapAfpsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/afps")
                .WithTags("Afps (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateAfpCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/afps/{result.Code}", result);
            })
            .WithName("CreateAfp")
            .RequireAuthorization("AFPS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetAfpsPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAfpsPagedQueryString")
            .RequireAuthorization("AFPS.VIEW");

            group.MapPost("/paged/body", async (
                GetAfpsPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAfpsPagedBody")
            .RequireAuthorization("AFPS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetAfpByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetAfpByCode")
            .RequireAuthorization("AFPS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateAfpCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateAfp")
            .RequireAuthorization("AFPS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleAfpStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleAfpStatus")
            .RequireAuthorization("AFPS.UPDATE");
        }
    }
}