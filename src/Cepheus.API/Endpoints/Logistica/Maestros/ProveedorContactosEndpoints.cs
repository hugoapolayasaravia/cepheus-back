using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.CreateProveedorContacto;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.GetProveedorContactoById;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.GetProveedorContactosByProveedor;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.ToggleProveedorContactoStatus;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.UpdateProveedorContacto;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class ProveedorContactosEndpoints
    {
        public static void MapProveedorContactosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/proveedores/{codigoProveedor}/contactos")
                .WithTags("Contactos de Proveedor (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (string codigoProveedor, CreateProveedorContactoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoProveedor, command.ProveedorCode, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de proveedor de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/proveedores/{codigoProveedor}/contactos/{result.Id}", result);
            })
            .WithName("CreateProveedorContacto")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapGet("/", async (string codigoProveedor, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorContactosByProveedorQuery(codigoProveedor));
                return Results.Ok(result);
            })
            .WithName("GetProveedorContactosByProveedor")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapGet("/{id:int}", async (string codigoProveedor, int id, ISender sender) =>
            {
                var result = await sender.Send(new GetProveedorContactoByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetProveedorContactoById")
            .RequireAuthorization("PROVEEDORES.VIEW");

            group.MapPut("/{id:int}", async (string codigoProveedor, int id, UpdateProveedorContactoCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("El id de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateProveedorContacto")
            .RequireAuthorization("PROVEEDORES.UPDATE");

            group.MapPatch("/{id:int}/toggle-status", async (string codigoProveedor, int id, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleProveedorContactoStatusCommand(id));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleProveedorContactoStatus")
            .RequireAuthorization("PROVEEDORES.UPDATE");
        }
    }
}
