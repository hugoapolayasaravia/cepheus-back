using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.UpdateTransportista
{
    public class UpdateTransportistaCommandHandler : IRequestHandler<UpdateTransportistaCommand, TransportistaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTransportistaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TransportistaResponse> Handle(UpdateTransportistaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.Transportistas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Transportista {request.Code} no encontrado.");
            }

            var transportista = new Transportista
            {
                Code = request.Code,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                LegalName = request.LegalName.Trim(),
                TradeName = string.IsNullOrWhiteSpace(request.TradeName) ? null : request.TradeName.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim().ToUpperInvariant(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                MtcRegistrationNumber = string.IsNullOrWhiteSpace(request.MtcRegistrationNumber) ? null : request.MtcRegistrationNumber.Trim(),
                CertificationNumber = string.IsNullOrWhiteSpace(request.CertificationNumber) ? null : request.CertificationNumber.Trim(),
                IsOwnFleet = request.IsOwnFleet,
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.Transportistas.Update(transportista);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El transportista fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTransportista.CreateTransportistaCommandHandler.Map(transportista);
        }
    }
}
