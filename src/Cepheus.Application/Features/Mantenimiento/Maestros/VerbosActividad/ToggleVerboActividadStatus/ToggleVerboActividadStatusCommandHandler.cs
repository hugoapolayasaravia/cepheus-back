using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.ToggleVerboActividadStatus
{
    public class ToggleVerboActividadStatusCommandHandler : IRequestHandler<ToggleVerboActividadStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleVerboActividadStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleVerboActividadStatusCommand request, CancellationToken cancellationToken)
        {
            var verboActividad = await _uow.Mantenimiento.Maestros.VerbosActividad.Query()
                .FirstOrDefaultAsync(v => v.Code == request.Code, cancellationToken);

            if (verboActividad is null)
            {
                throw new KeyNotFoundException($"Verbo de actividad {request.Code} no encontrado.");
            }

            verboActividad.IsActive = !verboActividad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return verboActividad.IsActive;
        }
    }
}
