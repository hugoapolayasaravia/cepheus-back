using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.CreateRegimenPensionario;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.GetRegimenPensionarioByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.GetRegimenesPensionariosPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.ToggleRegimenPensionarioStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.UpdateRegimenPensionario;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class RegimenesPensionariosEndpoints
    {
        public static void MapRegimenesPensionariosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/regimenes-pensionarios")
                .WithTags("RegimenesPensionarios (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateRegimenPensionarioCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/regimenes-pensionarios/{result.Code}", result);
            })
            .WithName("CreateRegimenPensionario")
            .RequireAuthorization("REGIMENESPENSIONARIO.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetRegimenesPensionariosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRegimenesPensionariosPagedQueryString")
            .RequireAuthorization("REGIMENESPENSIONARIO.VIEW");

            group.MapPost("/paged/body", async (
                GetRegimenesPensionariosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetRegimenesPensionariosPagedBody")
            .RequireAuthorization("REGIMENESPENSIONARIO.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetRegimenPensionarioByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetRegimenPensionarioByCode")
            .RequireAuthorization("REGIMENESPENSIONARIO.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateRegimenPensionarioCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateRegimenPensionario")
            .RequireAuthorization("REGIMENESPENSIONARIO.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleRegimenPensionarioStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleRegimenPensionarioStatus")
            .RequireAuthorization("REGIMENESPENSIONARIO.UPDATE");
        }
    }
}