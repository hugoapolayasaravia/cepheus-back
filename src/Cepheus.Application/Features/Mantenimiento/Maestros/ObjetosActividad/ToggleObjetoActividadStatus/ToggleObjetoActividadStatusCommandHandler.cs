using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.ToggleObjetoActividadStatus
{
    public class ToggleObjetoActividadStatusCommandHandler : IRequestHandler<ToggleObjetoActividadStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleObjetoActividadStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleObjetoActividadStatusCommand request, CancellationToken cancellationToken)
        {
            var objetoActividad = await _uow.Mantenimiento.Maestros.ObjetosActividad.Query()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (objetoActividad is null)
            {
                throw new KeyNotFoundException($"Objeto de actividad {request.Code} no encontrado.");
            }

            objetoActividad.IsActive = !objetoActividad.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return objetoActividad.IsActive;
        }
    }
}
