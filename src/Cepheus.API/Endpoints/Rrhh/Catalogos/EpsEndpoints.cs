using Cepheus.Application.Features.Rrhh.Catalogos.Epss.CreateEps;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.GetEpsByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.GetEpsPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.ToggleEpsStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.UpdateEps;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class EpsEndpoints
    {
        public static void MapEpsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/eps")
                .WithTags("Eps (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateEpsCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/eps/{result.Code}", result);
            })
            .WithName("CreateEps")
            .RequireAuthorization("EPS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetEpsPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEpsPagedQueryString")
            .RequireAuthorization("EPS.VIEW");

            group.MapPost("/paged/body", async (
                GetEpsPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetEpsPagedBody")
            .RequireAuthorization("EPS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetEpsByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetEpsByCode")
            .RequireAuthorization("EPS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateEpsCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateEps")
            .RequireAuthorization("EPS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleEpsStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleEpsStatus")
            .RequireAuthorization("EPS.UPDATE");
        }
    }
}