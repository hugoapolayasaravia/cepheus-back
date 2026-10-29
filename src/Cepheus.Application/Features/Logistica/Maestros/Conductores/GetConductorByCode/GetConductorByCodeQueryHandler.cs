using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductorByCode
{
    public class GetConductorByCodeQueryHandler : IRequestHandler<GetConductorByCodeQuery, ConductorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetConductorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ConductorResponse> Handle(GetConductorByCodeQuery request, CancellationToken cancellationToken)
        {
            var conductor = await _uow.Logistica.Maestros.Conductores.Query()
                .AsNoTracking()
                .Where(c => c.Code == request.Code)
                .Select(c => new ConductorResponse
                {
                    Code = c.Code,
                    DocumentTypeCode = c.DocumentTypeCode,
                    DocumentNumber = c.DocumentNumber,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    DriverLicenseNumber = c.DriverLicenseNumber,
                    LicenseCategory = c.LicenseCategory,
                    Phone = c.Phone,
                    Email = c.Email,
                    Observations = c.Observations,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (conductor is null)
            {
                throw new KeyNotFoundException($"Conductor {request.Code} no encontrado.");
            }

            return conductor;
        }
    }
}
