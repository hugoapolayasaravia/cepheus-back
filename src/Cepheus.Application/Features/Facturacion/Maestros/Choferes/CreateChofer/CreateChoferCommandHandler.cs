using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.CreateChofer
{
    public class CreateChoferCommandHandler : IRequestHandler<CreateChoferCommand, ChoferResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateChoferCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChoferResponse> Handle(CreateChoferCommand request, CancellationToken cancellationToken)
        {
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();

            // Correlativo por transportista (PK compuesta TransportistaCode + Code)
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Maestros.Choferes.Query()
                    .Where(c => c.TransportistaCode == transportistaCode)
                    .Select(c => c.Code),
                length: 4, entityLabel: $"Choferes del transportista {transportistaCode}", cancellationToken);

            var chofer = new ChoferVenta
            {
                TransportistaCode = transportistaCode,
                Code = code,
                FullName = request.FullName.Trim(),
                DriverLicenseNumber = request.DriverLicenseNumber.Trim().ToUpperInvariant(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Maestros.Choferes.AddAsync(chofer, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(chofer);
        }

        internal static ChoferResponse Map(ChoferVenta chofer) => new()
        {
            TransportistaCode = chofer.TransportistaCode,
            Code = chofer.Code,
            FullName = chofer.FullName,
            DriverLicenseNumber = chofer.DriverLicenseNumber,
            Observations = chofer.Observations,
            IsActive = chofer.IsActive,
            CreatedAt = chofer.CreatedAt,
            UpdatedAt = chofer.UpdatedAt,
            RowVersion = chofer.RowVersion
        };
    }
}
