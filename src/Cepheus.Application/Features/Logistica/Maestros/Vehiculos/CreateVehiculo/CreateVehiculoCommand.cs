using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.CreateVehiculo
{
    public record CreateVehiculoCommand(
        string TransportistaCode,
        string LicensePlate,
        string? VehicleCategory,
        string? VehicleType,
        string? Brand,
        string? Model,
        short? ManufactureYear,
        string? EngineNumber,
        string? ChassisNumber,
        string? Color,
        decimal? CargoCapacityKg,
        decimal? LengthM,
        decimal? WidthM,
        decimal? HeightM,
        string? VehicularCertificateNumber,
        string? CirculationCardNumber,
        string? VehicularConfiguration,
        string? Observations
    ) : IRequest<VehiculoResponse>;
}
