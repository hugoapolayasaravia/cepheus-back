using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.CreateProveedorCondicion;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.GetProveedorCondicionById;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.GetProveedorCondicionesByProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.ToggleProveedorCondicionStatus;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.UpdateProveedorCondicion;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ProveedorCondicionesEndpoints
    {
        public static void MapProveedorCondicionesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/proveedores/{codigoProveedor}/condiciones")
                .WithTags("Condiciones Comerciales de Proveedor (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (string codigoProveedor, CreateProveedorCondicionCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoProveedor, command.ProveedorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de proveedor de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/proveedores/{codigoProveedor}/condiciones/{result.Id}", result);
            })
            .WithName("CreateProveedorCondicion")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapGet("/", async (string codigoProveedor, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorCondicionesByProveedorQuery(codigoProveedor));
                return Results.Ok(result);
            })
            .WithName("GetProveedorCondicionesByProveedor")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapGet("/{id:int}", async (string codigoProveedor, int id, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorCondicionByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetProveedorCondicionById")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapPut("/{id:int}", async (string codigoProveedor, int id, UpdateProveedorCondicionCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateProveedorCondicion")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapPatch("/{id:int}/toggle-status", async (string codigoProveedor, int id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleProveedorCondicionStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleProveedorCondicionStatus")
            .RequireAuthorization("PROVEEDORES.UPDATE");
        }
    }
}
