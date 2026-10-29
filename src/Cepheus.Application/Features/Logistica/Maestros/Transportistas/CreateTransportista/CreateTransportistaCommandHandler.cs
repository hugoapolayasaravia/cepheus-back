using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.CreateTransportista
{
    public class CreateTransportistaCommandHandler : IRequestHandler<CreateTransportistaCommand, TransportistaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTransportistaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TransportistaResponse> Handle(CreateTransportistaCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Maestros.Transportistas.Query().Select(t => t.Code), length: 5, entityLabel: "Transportistas", cancellationToken);

            var transportista = new Transportista
            {
                Code = code,
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
                IsActive = true
            };

            await _uow.Logistica.Maestros.Transportistas.AddAsync(transportista, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(transportista);
        }

        internal static TransportistaResponse Map(Transportista transportista) => new()
        {
            Code = transportista.Code,
            DocumentTypeCode = transportista.DocumentTypeCode,
            DocumentNumber = transportista.DocumentNumber,
            LegalName = transportista.LegalName,
            TradeName = transportista.TradeName,
            Address = transportista.Address,
            UbigeoCode = transportista.UbigeoCode,
            Phone = transportista.Phone,
            Email = transportista.Email,
            MtcRegistrationNumber = transportista.MtcRegistrationNumber,
            CertificationNumber = transportista.CertificationNumber,
            IsOwnFleet = transportista.IsOwnFleet,
            Observations = transportista.Observations,
            IsActive = transportista.IsActive,
            CreatedAt = transportista.CreatedAt,
            UpdatedAt = transportista.UpdatedAt,
            RowVersion = transportista.RowVersion
        };
    }
}
