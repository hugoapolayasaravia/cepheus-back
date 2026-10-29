using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.GetVehiculoByCode
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
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            var vehicleType = TipoVehiculoParser.ParseOrNotFound(request.VehicleType);
            var code = request.Code.Trim().ToUpperInvariant();

            var vehiculo = await _uow.Facturacion.Maestros.Vehiculos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(v =>
                    v.TransportistaCode == transportistaCode &&
                    v.VehicleType == vehicleType &&
                    v.Code == code, cancellationToken);

            if (vehiculo is null)
            {
                throw new KeyNotFoundException($"Vehículo {transportistaCode}/{vehicleType}/{code} no encontrado.");
            }

            return CreateVehiculo.CreateVehiculoCommandHandler.Map(vehiculo);
        }
    }
}
