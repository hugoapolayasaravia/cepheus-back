using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.ToggleActividadStatus
{
    public class ToggleActividadStatusCommandHandler : IRequestHandler<ToggleActividadStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleActividadStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleActividadStatusCommand request, CancellationToken cancellationToken)
        {
            var actividad = await _uow.Mantenimiento.Maestros.Actividades.Query()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (actividad is null)
            {
                throw new KeyNotFoundException($"Actividad {request.Code} no encontrada.");
            }

            actividad.IsActive = !actividad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return actividad.IsActive;
        }
    }
}
