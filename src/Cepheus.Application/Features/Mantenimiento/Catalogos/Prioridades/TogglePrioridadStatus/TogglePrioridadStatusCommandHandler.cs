using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.TogglePrioridadStatus
{
    public class TogglePrioridadStatusCommandHandler : IRequestHandler<TogglePrioridadStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public TogglePrioridadStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(TogglePrioridadStatusCommand request, CancellationToken cancellationToken)
        {
            var prioridad = await _uow.Mantenimiento.Catalogos.Prioridades.Query()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (prioridad is null)
            {
                throw new KeyNotFoundException($"Prioridad {request.Code} no encontrada.");
            }

            prioridad.IsActive = !prioridad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return prioridad.IsActive;
        }
    }
}
