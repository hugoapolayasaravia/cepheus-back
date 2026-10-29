using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.CreateVehiculo;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.GetVehiculoByCode;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.GetVehiculosPaginated;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.ToggleVehiculoStatus;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.UpdateVehiculo;
using MediatR;

namespace Cepheus.API.Endpoints.Logistica.Maestros
{
    public static class VehiculosEndpoints
    {
        public static void MapVehiculosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/logistica/maestros/vehiculos")
                .WithTags("Vehículos (Logística)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateVehiculoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/api/logistica/maestros/vehiculos/{result.Code}", result);
            })
            .WithName("CreateVehiculo")
            .RequireAuthorization("VEHICULOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetVehiculosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetVehiculosPagedQueryString")
            .RequireAuthorization("VEHICULOS.VIEW");

            group.MapPost("/paged/body", async (
                GetVehiculosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetVehiculosPagedBody")
            .RequireAuthorization("VEHICULOS.VIEW");

            group.MapGet("/{codigo}", async (string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetVehiculoByCodeQuery(codigo));
                return Results.Ok(result);
            })
            .WithName("GetVehiculoByCode")
            .RequireAuthorization("VEHICULOS.VIEW");

            group.MapPut("/{codigo}", async (string codigo, UpdateVehiculoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("El código de la ruta no coincide con el del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateVehiculo")
            .RequireAuthorization("VEHICULOS.UPDATE");

            group.MapPatch("/{codigo}/toggle-status", async (string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleVehiculoStatusCommand(codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleVehiculoStatus")
            .RequireAuthorization("VEHICULOS.UPDATE");
        }
    }
}
