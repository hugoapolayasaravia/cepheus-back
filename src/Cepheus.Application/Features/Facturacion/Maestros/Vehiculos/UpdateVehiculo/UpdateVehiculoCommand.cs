using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.UpdateVehiculo
{
    public record UpdateVehiculoCommand(
        string TransportistaCode,
        string VehicleType,
        string Code,
        string LicensePlate,
        string? Brand,
        string? Model,
        string? ChoferCode,
        decimal Capacity,
        decimal Suple,
        decimal LengthM,
        decimal WidthM,
        decimal HeightM,
        decimal Telescopic,
        decimal WithoutSuple,
        decimal WithSuple,
        decimal CubicWithoutSuple,
        decimal CubicWithSuple,
        string? MtcInternalCode,
        string? VehicularConfiguration,
        string? PlanillaCode,
        string? Observations,
        byte[] RowVersion
    ) : IRequest<VehiculoResponse>;
}
