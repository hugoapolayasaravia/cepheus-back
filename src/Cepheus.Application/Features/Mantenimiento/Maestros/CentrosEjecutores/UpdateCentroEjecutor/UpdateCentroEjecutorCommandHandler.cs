using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.UpdateCentroEjecutor
{
    public class UpdateCentroEjecutorCommandHandler : IRequestHandler<UpdateCentroEjecutorCommand, CentroEjecutorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCentroEjecutorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CentroEjecutorResponse> Handle(UpdateCentroEjecutorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Centro ejecutor {request.Code} no encontrado.");
            }

            var centroEjecutor = new CentroEjecutor
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Maestros.CentrosEjecutores.Update(centroEjecutor);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El centro ejecutor fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new CentroEjecutorResponse
            {
                Code = centroEjecutor.Code,
                Name = centroEjecutor.Name,
                IsActive = centroEjecutor.IsActive,
                CreatedAt = centroEjecutor.CreatedAt,
                UpdatedAt = centroEjecutor.UpdatedAt,
                RowVersion = centroEjecutor.RowVersion
            };
        }
    }
}
