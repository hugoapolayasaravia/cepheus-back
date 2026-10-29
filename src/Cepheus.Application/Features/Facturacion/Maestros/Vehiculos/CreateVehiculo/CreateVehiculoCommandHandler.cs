using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.CreateVehiculo
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
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            var vehicleType = TipoVehiculoParser.ParseOrNotFound(request.VehicleType);

            // Correlativo por transportista y tipo (PK compuesta)
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Maestros.Vehiculos.Query()
                    .Where(v => v.TransportistaCode == transportistaCode && v.VehicleType == vehicleType)
                    .Select(v => v.Code),
                length: 4, entityLabel: $"Vehículos del transportista {transportistaCode}", cancellationToken);

            var vehiculo = new VehiculoVenta
            {
                TransportistaCode = transportistaCode,
                VehicleType = vehicleType,
                Code = code,
                LicensePlate = request.LicensePlate.Trim().ToUpperInvariant(),
                Brand = string.IsNullOrWhiteSpace(request.Brand) ? null : request.Brand.Trim(),
                Model = string.IsNullOrWhiteSpace(request.Model) ? null : request.Model.Trim(),
                ChoferCode = string.IsNullOrWhiteSpace(request.ChoferCode) ? null : request.ChoferCode.Trim().ToUpperInvariant(),
                Capacity = request.Capacity,
                Suple = request.Suple,
                LengthM = request.LengthM,
                WidthM = request.WidthM,
                HeightM = request.HeightM,
                Telescopic = request.Telescopic,
                WithoutSuple = request.WithoutSuple,
                WithSuple = request.WithSuple,
                CubicWithoutSuple = request.CubicWithoutSuple,
                CubicWithSuple = request.CubicWithSuple,
                MtcInternalCode = string.IsNullOrWhiteSpace(request.MtcInternalCode) ? null : request.MtcInternalCode.Trim(),
                VehicularConfiguration = string.IsNullOrWhiteSpace(request.VehicularConfiguration) ? null : request.VehicularConfiguration.Trim(),
                PlanillaCode = string.IsNullOrWhiteSpace(request.PlanillaCode) ? null : request.PlanillaCode.Trim(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Maestros.Vehiculos.AddAsync(vehiculo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(vehiculo);
        }

        internal static VehiculoResponse Map(VehiculoVenta v) => new()
        {
            TransportistaCode = v.TransportistaCode,
            VehicleType = v.VehicleType.ToString(),
            Code = v.Code,
            LicensePlate = v.LicensePlate,
            Brand = v.Brand,
            Model = v.Model,
            ChoferCode = v.ChoferCode,
            Capacity = v.Capacity,
            Suple = v.Suple,
            LengthM = v.LengthM,
            WidthM = v.WidthM,
            HeightM = v.HeightM,
            Telescopic = v.Telescopic,
            WithoutSuple = v.WithoutSuple,
            WithSuple = v.WithSuple,
            CubicWithoutSuple = v.CubicWithoutSuple,
            CubicWithSuple = v.CubicWithSuple,
            MtcInternalCode = v.MtcInternalCode,
            VehicularConfiguration = v.VehicularConfiguration,
            PlanillaCode = v.PlanillaCode,
            Observations = v.Observations,
            ApprovedBy = v.ApprovedBy,
            ApprovedAt = v.ApprovedAt,
            IsActive = v.IsActive,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt,
            CreatedBy = v.CreatedBy,
            UpdatedBy = v.UpdatedBy,
            RowVersion = v.RowVersion
        };
    }
}
