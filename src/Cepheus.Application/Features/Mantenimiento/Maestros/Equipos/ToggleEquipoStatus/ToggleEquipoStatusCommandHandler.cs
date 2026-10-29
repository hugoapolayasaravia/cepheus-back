using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.ToggleEquipoStatus
{
    public class ToggleEquipoStatusCommandHandler : IRequestHandler<ToggleEquipoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleEquipoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleEquipoStatusCommand request, CancellationToken cancellationToken)
        {
            var equipo = await _uow.Mantenimiento.Maestros.Equipos.Query()
                .FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);

            if (equipo is null)
            {
                throw new KeyNotFoundException($"Equipo {request.Code} no encontrado.");
            }

            equipo.IsActive = !equipo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return equipo.IsActive;
        }
    }
}
