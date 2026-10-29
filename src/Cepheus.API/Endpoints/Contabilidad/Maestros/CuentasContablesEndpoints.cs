using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.CreateCuentaContable;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.GetCuentaContableByCode;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.GetCuentasContablesPaginated;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.ToggleCuentaContableStatus;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.UpdateCuentaContable;
using MediatR;

namespace Cepheus.API.Endpoints.Contabilidad.Maestros
{
    public static class CuentasContablesEndpoints
    {
        public static void MapCuentasContablesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/contabilidad/maestros/cuentas-contables")
                .WithTags("Cuentas contables")
                .RequireAuthorization();

            group.MapPost("/", async (CreateCuentaContableCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/contabilidad/maestros/cuentas-contables/{result.Code}", result);
            })
            .WithName("CreateCuentaContable")
            .RequireAuthorization("CUENTASCONTABLES.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetCuentasContablesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCuentasContablesPagedQueryString")
            .RequireAuthorization("CUENTASCONTABLES.VIEW");

            group.MapPost("/paged/body", async (
                GetCuentasContablesPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetCuentasContablesPagedBody")
            .RequireAuthorization("CUENTASCONTABLES.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetCuentaContableByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetCuentaContableByCode")
            .RequireAuthorization("CUENTASCONTABLES.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateCuentaContableCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateCuentaContable")
            .RequireAuthorization("CUENTASCONTABLES.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleCuentaContableStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleCuentaContableStatus")
            .RequireAuthorization("CUENTASCONTABLES.UPDATE");
        }
    }
}
