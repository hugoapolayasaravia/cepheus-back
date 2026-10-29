using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.GetTransportistaByCode
{
    public class GetTransportistaByCodeQueryHandler : IRequestHandler<GetTransportistaByCodeQuery, TransportistaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTransportistaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TransportistaResponse> Handle(GetTransportistaByCodeQuery request, CancellationToken cancellationToken)
        {
            var transportista = await _uow.Logistica.Maestros.Transportistas.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TransportistaResponse
                {
                    Code = t.Code,
                    DocumentTypeCode = t.DocumentTypeCode,
                    DocumentNumber = t.DocumentNumber,
                    LegalName = t.LegalName,
                    TradeName = t.TradeName,
                    Address = t.Address,
                    UbigeoCode = t.UbigeoCode,
                    Phone = t.Phone,
                    Email = t.Email,
                    MtcRegistrationNumber = t.MtcRegistrationNumber,
                    CertificationNumber = t.CertificationNumber,
                    IsOwnFleet = t.IsOwnFleet,
                    Observations = t.Observations,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (transportista is null)
            {
                throw new KeyNotFoundException($"Transportista {request.Code} no encontrado.");
            }

            return transportista;
        }
    }
}
