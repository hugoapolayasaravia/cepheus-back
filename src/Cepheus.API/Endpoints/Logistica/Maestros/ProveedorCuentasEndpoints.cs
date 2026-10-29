using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.CreateProveedorCuenta;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.GetProveedorCuentaById;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.GetProveedorCuentasByProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.ToggleProveedorCuentaStatus;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.UpdateProveedorCuenta;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ProveedorCuentasEndpoints
    {
        public static void MapProveedorCuentasEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/proveedores/{codigoProveedor}/cuentas")
                .WithTags("Cuentas Bancarias de Proveedor (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (string codigoProveedor, CreateProveedorCuentaCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoProveedor, command.ProveedorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de proveedor de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/proveedores/{codigoProveedor}/cuentas/{result.Id}", result);
            })
            .WithName("CreateProveedorCuenta")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapGet("/", async (string codigoProveedor, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorCuentasByProveedorQuery(codigoProveedor));
                return Results.Ok(result);
            })
            .WithName("GetProveedorCuentasByProveedor")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapGet("/{id:int}", async (string codigoProveedor, int id, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorCuentaByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetProveedorCuentaById")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapPut("/{id:int}", async (string codigoProveedor, int id, UpdateProveedorCuentaCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateProveedorCuenta")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapPatch("/{id:int}/toggle-status", async (string codigoProveedor, int id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleProveedorCuentaStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleProveedorCuentaStatus")
            .RequireAuthorization("PROVEEDORES.UPDATE");
        }
    }
}
