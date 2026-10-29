using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.CreateRegimenLaboral;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.GetRegimenLaboralByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.GetRegimenesLaboralesPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.ToggleRegimenLaboralStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.UpdateRegimenLaboral;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class RegimenesLaboralesEndpoints
    {
        public static void MapRegimenesLaboralesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/regimenes-laborales")
                .WithTags("RegimenesLaborales (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateRegimenLaboralCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/regimenes-laborales/{result.Code}", result);
            })
            .WithName("CreateRegimenLaboral")
            .RequireAuthorization("REGIMENESLABORALES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetRegimenesLaboralesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRegimenesLaboralesPagedQueryString")
            .RequireAuthorization("REGIMENESLABORALES.VIEW");

            group.MapPost("/paged/body", async (
                GetRegimenesLaboralesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRegimenesLaboralesPagedBody")
            .RequireAuthorization("REGIMENESLABORALES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetRegimenLaboralByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetRegimenLaboralByCode")
            .RequireAuthorization("REGIMENESLABORALES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateRegimenLaboralCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateRegimenLaboral")
            .RequireAuthorization("REGIMENESLABORALES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleRegimenLaboralStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleRegimenLaboralStatus")
            .RequireAuthorization("REGIMENESLABORALES.UPDATE");
        }
    }
}