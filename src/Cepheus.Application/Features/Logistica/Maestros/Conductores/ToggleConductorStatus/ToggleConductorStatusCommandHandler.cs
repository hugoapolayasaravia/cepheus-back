using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.ToggleConductorStatus
{
    public class ToggleConductorStatusCommandHandler : IRequestHandler<ToggleConductorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleConductorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleConductorStatusCommand request, CancellationToken cancellationToken)
        {
            var conductor = await _uow.Logistica.Maestros.Conductores.Query()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (conductor is null)
            {
                throw new KeyNotFoundException($"Conductor {request.Code} no encontrado.");
            }

            conductor.IsActive = !conductor.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return conductor.IsActive;
        }
    }
}
