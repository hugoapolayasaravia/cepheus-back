using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.CreateParentesco;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.GetParentescoByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.GetParentescosPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.ToggleParentescoStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.UpdateParentesco;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class ParentescosEndpoints
    {
        public static void MapParentescosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/parentescos")
                .WithTags("Parentescos (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateParentescoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/parentescos/{result.Code}", result);
            })
            .WithName("CreateParentesco")
            .RequireAuthorization("PARENTESCOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetParentescosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetParentescosPagedQueryString")
            .RequireAuthorization("PARENTESCOS.VIEW");

            group.MapPost("/paged/body", async (
                GetParentescosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetParentescosPagedBody")
            .RequireAuthorization("PARENTESCOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetParentescoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetParentescoByCode")
            .RequireAuthorization("PARENTESCOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateParentescoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateParentesco")
            .RequireAuthorization("PARENTESCOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleParentescoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleParentescoStatus")
            .RequireAuthorization("PARENTESCOS.UPDATE");
        }
    }
}