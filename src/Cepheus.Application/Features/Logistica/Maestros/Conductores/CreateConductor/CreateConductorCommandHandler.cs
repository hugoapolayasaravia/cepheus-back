using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.CreateConductor
{
    public class CreateConductorCommandHandler : IRequestHandler<CreateConductorCommand, ConductorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateConductorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ConductorResponse> Handle(CreateConductorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Maestros.Conductores.Query().Select(c => c.Code), length: 5, entityLabel: "Conductores", cancellationToken);

            var conductor = new Conductor
            {
                Code = code,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                DriverLicenseNumber = request.DriverLicenseNumber.Trim().ToUpperInvariant(),
                LicenseCategory = string.IsNullOrWhiteSpace(request.LicenseCategory) ? null : request.LicenseCategory.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Maestros.Conductores.AddAsync(conductor, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(conductor);
        }

        internal static ConductorResponse Map(Conductor conductor) => new()
        {
            Code = conductor.Code,
            DocumentTypeCode = conductor.DocumentTypeCode,
            DocumentNumber = conductor.DocumentNumber,
            FirstName = conductor.FirstName,
            LastName = conductor.LastName,
            DriverLicenseNumber = conductor.DriverLicenseNumber,
            LicenseCategory = conductor.LicenseCategory,
            Phone = conductor.Phone,
            Email = conductor.Email,
            Observations = conductor.Observations,
            IsActive = conductor.IsActive,
            CreatedAt = conductor.CreatedAt,
            UpdatedAt = conductor.UpdatedAt,
            RowVersion = conductor.RowVersion
        };
    }
}
