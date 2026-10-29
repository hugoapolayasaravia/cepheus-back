using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.CreateTipoCuenta;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.GetTipoCuentaByCode;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.GetTiposCuentaPaginated;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.ToggleTipoCuentaStatus;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.UpdateTipoCuenta;
using MediatR;

namespace Cepheus.API.Endpoints.Rrhh.Catalogos
{
    public static class TiposCuentaEndpoints
    {
        public static void MapTiposCuentaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/rrhh/catalogos/tipos-cuenta")
                .WithTags("TiposCuenta (RRHH)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateTipoCuentaCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/rrhh/catalogos/tipos-cuenta/{result.Code}", result);
            })
            .WithName("CreateTipoCuenta")
            .RequireAuthorization("TIPOSCUENTA.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetTiposCuentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCuentaPagedQueryString")
            .RequireAuthorization("TIPOSCUENTA.VIEW");

            group.MapPost("/paged/body", async (
                GetTiposCuentaPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTiposCuentaPagedBody")
            .RequireAuthorization("TIPOSCUENTA.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetTipoCuentaByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetTipoCuentaByCode")
            .RequireAuthorization("TIPOSCUENTA.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateTipoCuentaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateTipoCuenta")
            .RequireAuthorization("TIPOSCUENTA.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleTipoCuentaStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleTipoCuentaStatus")
            .RequireAuthorization("TIPOSCUENTA.UPDATE");
        }
    }
}