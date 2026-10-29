using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.CreateProveedorCuentaContable;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.DeleteProveedorCuentaContable;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.GetProveedorCuentaContableByKey;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.GetProveedorCuentasContablesPaginated;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.UpdateProveedorCuentaContable;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ProveedorCuentasContablesEndpoints
    {
        public static void MapProveedorCuentasContablesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/proveedor-cuentas-contables")
                .WithTags("Proveedor - Cuenta contable por pagar (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateProveedorCuentaContableCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/logistica/maestros/proveedor-cuentas-contables/{result.ProveedorCode}/{result.MonedaCode}", result);
            })
            .WithName("CreateProveedorCuentaContable")
            .RequireAuthorization("PROVEEDORCUENTASCONTABLES.CREATE");

            group.MapGet("/paged", async ([AsParameters] GetProveedorCuentasContablesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetProveedorCuentasContablesPagedQueryString")
            .RequireAuthorization("PROVEEDORCUENTASCONTABLES.VIEW");

            group.MapPost("/paged/body", async (GetProveedorCuentasContablesPaginatedQuery query, ISender sender) =>
                Results.Ok(await sender.Send(query)))
            .WithName("GetProveedorCuentasContablesPagedBody")
            .RequireAuthorization("PROVEEDORCUENTASCONTABLES.VIEW");

            group.MapGet("/{proveedorCode}/{monedaCode}", async (
                string proveedorCode, string monedaCode, ISender sender) =>
                Results.Ok(await sender.Send(new GetProveedorCuentaContableByKeyQuery(proveedorCode, monedaCode))))
            .WithName("GetProveedorCuentaContableByKey")
            .RequireAuthorization("PROVEEDORCUENTASCONTABLES.VIEW");

            group.MapPut("/{proveedorCode}/{monedaCode}", async (
                string proveedorCode, string monedaCode, UpdateProveedorCuentaContableCommand command, ISender sender) =>
            {
                if (!string.Equals(proveedorCode, command.ProveedorCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(monedaCode, command.MonedaCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }
                return Results.Ok(await sender.Send(command));
            })
            .WithName("UpdateProveedorCuentaContable")
            .RequireAuthorization("PROVEEDORCUENTASCONTABLES.UPDATE");

            group.MapDelete("/{proveedorCode}/{monedaCode}", async (
                string proveedorCode, string monedaCode, ISender sender) =>
            {
                await sender.Send(new DeleteProveedorCuentaContableCommand(proveedorCode, monedaCode));
                return Results.NoContent();
            })
            .WithName("DeleteProveedorCuentaContable")
            .RequireAuthorization("PROVEEDORCUENTASCONTABLES.DELETE");
        }
    }
}
