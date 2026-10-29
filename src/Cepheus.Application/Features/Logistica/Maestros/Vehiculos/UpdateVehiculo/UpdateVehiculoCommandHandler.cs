using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.UpdateVehiculo
{
    public class UpdateVehiculoCommandHandler : IRequestHandler<UpdateVehiculoCommand, VehiculoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateVehiculoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VehiculoResponse> Handle(UpdateVehiculoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.Vehiculos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Vehículo {request.Code} no encontrado.");
            }

            var vehiculo = new Vehiculo
            {
                Code = request.Code,
                TransportistaCode = current.TransportistaCode,
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

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.Vehiculos.Update(vehiculo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El vehículo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateVehiculo.CreateVehiculoCommandHandler.Map(vehiculo);
        }
    }
}
