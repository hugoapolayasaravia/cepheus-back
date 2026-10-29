using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.UpdateVehiculo
{
    // TransportistaCode NO se edita: reasignar un vehículo a otro
    // transportista es una operación estructural distinta (mismo criterio
    // que SubFamilia.FamiliaCode).
    public record UpdateVehiculoCommand(
        string Code,
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
        string? Observations,
        byte[] RowVersion
    ) : IRequest<VehiculoResponse>;
}
