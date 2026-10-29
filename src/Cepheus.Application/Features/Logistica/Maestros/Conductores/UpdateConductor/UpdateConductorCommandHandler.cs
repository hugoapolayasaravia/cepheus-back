using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.UpdateConductor
{
    public class UpdateConductorCommandHandler : IRequestHandler<UpdateConductorCommand, ConductorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateConductorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ConductorResponse> Handle(UpdateConductorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.Conductores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Conductor {request.Code} no encontrado.");
            }

            var conductor = new Conductor
            {
                Code = request.Code,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                DriverLicenseNumber = request.DriverLicenseNumber.Trim().ToUpperInvariant(),
                LicenseCategory = string.IsNullOrWhiteSpace(request.LicenseCategory) ? null : request.LicenseCategory.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.Conductores.Update(conductor);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El conductor fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateConductor.CreateConductorCommandHandler.Map(conductor);
        }
    }
}
