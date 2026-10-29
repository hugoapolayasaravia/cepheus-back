using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.GetVehiculoByCode
{
    public class GetVehiculoByCodeQueryHandler : IRequestHandler<GetVehiculoByCodeQuery, VehiculoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetVehiculoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VehiculoResponse> Handle(GetVehiculoByCodeQuery request, CancellationToken cancellationToken)
        {
            var vehiculo = await _uow.Logistica.Maestros.Vehiculos.Query()
                .AsNoTracking()
                .Where(v => v.Code == request.Code)
                .Select(v => new VehiculoResponse
                {
                    Code = v.Code,
                    TransportistaCode = v.TransportistaCode,
                    LicensePlate = v.LicensePlate,
                    VehicleCategory = v.VehicleCategory,
                    VehicleType = v.VehicleType,
                    Brand = v.Brand,
                    Model = v.Model,
                    ManufactureYear = v.ManufactureYear,
                    EngineNumber = v.EngineNumber,
                    ChassisNumber = v.ChassisNumber,
                    Color = v.Color,
                    CargoCapacityKg = v.CargoCapacityKg,
                    LengthM = v.LengthM,
                    WidthM = v.WidthM,
                    HeightM = v.HeightM,
                    VehicularCertificateNumber = v.VehicularCertificateNumber,
                    CirculationCardNumber = v.CirculationCardNumber,
                    VehicularConfiguration = v.VehicularConfiguration,
                    Observations = v.Observations,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    RowVersion = v.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (vehiculo is null)
            {
                throw new KeyNotFoundException($"Vehículo {request.Code} no encontrado.");
            }

            return vehiculo;
        }
    }
}
