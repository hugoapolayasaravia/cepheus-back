using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.CreateVehiculo
{
    public class CreateVehiculoCommandHandler : IRequestHandler<CreateVehiculoCommand, VehiculoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateVehiculoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VehiculoResponse> Handle(CreateVehiculoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Maestros.Vehiculos.Query().Select(v => v.Code), length: 5, entityLabel: "Vehículos", cancellationToken);

            var vehiculo = new Vehiculo
            {
                Code = code,
                TransportistaCode = request.TransportistaCode.Trim().ToUpperInvariant(),
                LicensePlate = request.LicensePlate.Trim().ToUpperInvariant(),
                VehicleCategory = string.IsNullOrWhiteSpace(request.VehicleCategory) ? null : request.VehicleCategory.Trim(),
                VehicleType = string.IsNullOrWhiteSpace(request.VehicleType) ? null : request.VehicleType.Trim(),
                Brand = string.IsNullOrWhiteSpace(request.Brand) ? null : request.Brand.Trim(),
                Model = string.IsNullOrWhiteSpace(request.Model) ? null : request.Model.Trim(),
                ManufactureYear = request.ManufactureYear,
                EngineNumber = string.IsNullOrWhiteSpace(request.EngineNumber) ? null : request.EngineNumber.Trim(),
                ChassisNumber = string.IsNullOrWhiteSpace(request.ChassisNumber) ? null : request.ChassisNumber.Trim(),
                Color = string.IsNullOrWhiteSpace(request.Color) ? null : request.Color.Trim(),
                CargoCapacityKg = request.CargoCapacityKg,
                LengthM = request.LengthM,
                WidthM = request.WidthM,
                HeightM = request.HeightM,
                VehicularCertificateNumber = string.IsNullOrWhiteSpace(request.VehicularCertificateNumber) ? null : request.VehicularCertificateNumber.Trim(),
                CirculationCardNumber = string.IsNullOrWhiteSpace(request.CirculationCardNumber) ? null : request.CirculationCardNumber.Trim(),
                VehicularConfiguration = string.IsNullOrWhiteSpace(request.VehicularConfiguration) ? null : request.VehicularConfiguration.Trim(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Maestros.Vehiculos.AddAsync(vehiculo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(vehiculo);
        }

        internal static VehiculoResponse Map(Vehiculo vehiculo) => new()
        {
            Code = vehiculo.Code,
            TransportistaCode = vehiculo.TransportistaCode,
            LicensePlate = vehiculo.LicensePlate,
            VehicleCategory = vehiculo.VehicleCategory,
            VehicleType = vehiculo.VehicleType,
            Brand = vehiculo.Brand,
            Model = vehiculo.Model,
            ManufactureYear = vehiculo.ManufactureYear,
            EngineNumber = vehiculo.EngineNumber,
            ChassisNumber = vehiculo.ChassisNumber,
            Color = vehiculo.Color,
            CargoCapacityKg = vehiculo.CargoCapacityKg,
            LengthM = vehiculo.LengthM,
            WidthM = vehiculo.WidthM,
            HeightM = vehiculo.HeightM,
            VehicularCertificateNumber = vehiculo.VehicularCertificateNumber,
            CirculationCardNumber = vehiculo.CirculationCardNumber,
            VehicularConfiguration = vehiculo.VehicularConfiguration,
            Observations = vehiculo.Observations,
            IsActive = vehiculo.IsActive,
            CreatedAt = vehiculo.CreatedAt,
            UpdatedAt = vehiculo.UpdatedAt,
            RowVersion = vehiculo.RowVersion
        };
    }
}
