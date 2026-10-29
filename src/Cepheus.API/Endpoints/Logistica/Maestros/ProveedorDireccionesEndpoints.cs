using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.CreateProveedorDireccion;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.GetProveedorDireccionById;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.GetProveedorDireccionesByProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.ToggleProveedorDireccionStatus;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.UpdateProveedorDireccion;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ProveedorDireccionesEndpoints
    {
        public static void MapProveedorDireccionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/proveedores/{codigoProveedor}/direcciones")
                .WithTags("Direcciones de Proveedor (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (string codigoProveedor, CreateProveedorDireccionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoProveedor, command.ProveedorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de proveedor de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/proveedores/{codigoProveedor}/direcciones/{result.Id}", result);
            })
            .WithName("CreateProveedorDireccion")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapGet("/", async (string codigoProveedor, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorDireccionesByProveedorQuery(codigoProveedor));
                return Results.Ok(result);
            })
            .WithName("GetProveedorDireccionesByProveedor")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapGet("/{id:int}", async (string codigoProveedor, int id, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorDireccionByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetProveedorDireccionById")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapPut("/{id:int}", async (string codigoProveedor, int id, UpdateProveedorDireccionCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateProveedorDireccion")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapPatch("/{id:int}/toggle-status", async (string codigoProveedor, int id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleProveedorDireccionStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleProveedorDireccionStatus")
            .RequireAuthorization("PROVEEDORES.UPDATE");
        }
    }
}
