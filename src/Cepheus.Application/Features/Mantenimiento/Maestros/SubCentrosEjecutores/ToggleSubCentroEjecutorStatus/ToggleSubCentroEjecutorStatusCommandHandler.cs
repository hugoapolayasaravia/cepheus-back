using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.ToggleSubCentroEjecutorStatus
{
    public class ToggleSubCentroEjecutorStatusCommandHandler
        : IRequestHandler<ToggleSubCentroEjecutorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSubCentroEjecutorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSubCentroEjecutorStatusCommand request, CancellationToken cancellationToken)
        {
            var subCentroEjecutor = await _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (subCentroEjecutor is null)
            {
                throw new KeyNotFoundException($"Subcentro ejecutor {request.Code} no encontrado.");
            }

            subCentroEjecutor.IsActive = !subCentroEjecutor.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return subCentroEjecutor.IsActive;
        }
    }
}
