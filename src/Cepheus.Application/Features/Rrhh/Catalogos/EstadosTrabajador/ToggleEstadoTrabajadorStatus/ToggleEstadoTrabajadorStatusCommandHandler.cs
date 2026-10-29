using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.ToggleEstadoTrabajadorStatus
{
    public class ToggleEstadoTrabajadorStatusCommandHandler : IRequestHandler<ToggleEstadoTrabajadorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleEstadoTrabajadorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleEstadoTrabajadorStatusCommand request, CancellationToken cancellationToken)
        {
            var estadoTrabajador = await _uow.Rrhh.Catalogos.EstadosTrabajador.Query()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (estadoTrabajador is null)
            {
                throw new KeyNotFoundException($"Estado de trabajador {request.Code} no encontrado.");
            }

            estadoTrabajador.IsActive = !estadoTrabajador.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return estadoTrabajador.IsActive;
        }
    }
}