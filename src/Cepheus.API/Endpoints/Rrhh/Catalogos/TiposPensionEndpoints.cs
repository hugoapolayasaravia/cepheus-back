using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.CreateTipoPension;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.GetTipoPensionByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.GetTiposPensionPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.ToggleTipoPensionStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.UpdateTipoPension;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposPensionEndpoints
    {
        public static void MapTiposPensionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-pension")
                .WithTags("TiposPension (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoPensionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-pension/{result.Code}", result);
            })
            .WithName("CreateTipoPension")
            .RequireAuthorization("TIPOSPENSION.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposPensionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposPensionPagedQueryString")
            .RequireAuthorization("TIPOSPENSION.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposPensionPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposPensionPagedBody")
            .RequireAuthorization("TIPOSPENSION.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoPensionByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoPensionByCode")
            .RequireAuthorization("TIPOSPENSION.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoPensionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoPension")
            .RequireAuthorization("TIPOSPENSION.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoPensionStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoPensionStatus")
            .RequireAuthorization("TIPOSPENSION.UPDATE");
        }
    }
}