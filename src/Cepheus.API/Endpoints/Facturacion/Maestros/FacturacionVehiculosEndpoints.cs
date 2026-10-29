using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.CreateVehiculo;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.GetVehiculoByCode;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.GetVehiculosPaginated;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.ToggleVehiculoStatus;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.UpdateVehiculo;
using MediatR;

namespace Cepheus.API.Endpoints.Facturacion.Maestros
{
    public static class FacturacionVehiculosEndpoints
    {
        public static void MapFacturacionVehiculosEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/facturacion/maestros/vehiculos")
                .WithTags("Vehículos (Facturación)")
                .RequireAuthorization();

            group.MapPost("/", async (CreateVehiculoCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created(
                    $"/api/facturacion/maestros/vehiculos/{result.TransportistaCode}/{result.VehicleType}/{result.Code}", result);
            })
            .WithName("CreateFacturacionVehiculo")
            .RequireAuthorization("FAC_VEHICULOS.CREATE");

            group.MapGet("/paged", async (
                [AsParameters] GetVehiculosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionVehiculosPagedQueryString")
            .RequireAuthorization("FAC_VEHICULOS.VIEW");

            group.MapPost("/paged/body", async (
                GetVehiculosPaginatedQuery query,
                ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetFacturacionVehiculosPagedBody")
            .RequireAuthorization("FAC_VEHICULOS.VIEW");

            group.MapGet("/{codigoTransportista}/{tipoVehiculo}/{codigo}", async (
                string codigoTransportista, string tipoVehiculo, string codigo, ISender sender) =>
            {
                var result = await sender.Send(new GetVehiculoByCodeQuery(codigoTransportista, tipoVehiculo, codigo));
                return Results.Ok(result);
            })
            .WithName("GetFacturacionVehiculoByCode")
            .RequireAuthorization("FAC_VEHICULOS.VIEW");

            group.MapPut("/{codigoTransportista}/{tipoVehiculo}/{codigo}", async (
                string codigoTransportista, string tipoVehiculo, string codigo,
                UpdateVehiculoCommand command, ISender sender) =>
            {
                if (!string.Equals(codigoTransportista, command.TransportistaCode, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(tipoVehiculo, command.VehicleType, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(codigo, command.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest("La clave de la ruta no coincide con la del cuerpo.");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("UpdateFacturacionVehiculo")
            .RequireAuthorization("FAC_VEHICULOS.UPDATE");

            group.MapPatch("/{codigoTransportista}/{tipoVehiculo}/{codigo}/toggle-status", async (
                string codigoTransportista, string tipoVehiculo, string codigo, ISender sender) =>
            {
                var isActive = await sender.Send(new ToggleVehiculoStatusCommand(codigoTransportista, tipoVehiculo, codigo));
                return Results.Ok(new { IsActive = isActive });
            })
            .WithName("ToggleFacturacionVehiculoStatus")
            .RequireAuthorization("FAC_VEHICULOS.UPDATE");
        }
    }
}
