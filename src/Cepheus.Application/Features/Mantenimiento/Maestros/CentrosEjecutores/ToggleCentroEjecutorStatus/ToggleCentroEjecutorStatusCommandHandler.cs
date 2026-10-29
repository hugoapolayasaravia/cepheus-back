using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.ToggleCentroEjecutorStatus
{
    public class ToggleCentroEjecutorStatusCommandHandler : IRequestHandler<ToggleCentroEjecutorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCentroEjecutorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCentroEjecutorStatusCommand request, CancellationToken cancellationToken)
        {
            var centroEjecutor = await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (centroEjecutor is null)
            {
                throw new KeyNotFoundException($"Centro ejecutor {request.Code} no encontrado.");
            }

            centroEjecutor.IsActive = !centroEjecutor.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return centroEjecutor.IsActive;
        }
    }
}
