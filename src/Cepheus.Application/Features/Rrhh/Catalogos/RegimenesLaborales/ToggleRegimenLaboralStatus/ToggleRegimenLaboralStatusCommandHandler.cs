using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.ToggleRegimenLaboralStatus
{
    public class ToggleRegimenLaboralStatusCommandHandler : IRequestHandler<ToggleRegimenLaboralStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleRegimenLaboralStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleRegimenLaboralStatusCommand request, CancellationToken cancellationToken)
        {
            var regimenLaboral = await _uow.Rrhh.Catalogos.RegimenesLaborales.Query()
                .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);

            if (regimenLaboral is null)
            {
                throw new KeyNotFoundException($"Régimen laboral {request.Code} no encontrado.");
            }

            regimenLaboral.IsActive = !regimenLaboral.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return regimenLaboral.IsActive;
        }
    }
}