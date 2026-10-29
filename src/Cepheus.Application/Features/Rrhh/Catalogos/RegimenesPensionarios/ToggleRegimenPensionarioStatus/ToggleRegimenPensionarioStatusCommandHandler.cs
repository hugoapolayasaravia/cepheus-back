using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.ToggleRegimenPensionarioStatus
{
    public class ToggleRegimenPensionarioStatusCommandHandler
        : IRequestHandler<ToggleRegimenPensionarioStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleRegimenPensionarioStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleRegimenPensionarioStatusCommand request, CancellationToken cancellationToken)
        {
            var regimenPensionario = await _uow.Rrhh.Catalogos.RegimenesPensionarios.Query()
                .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);

            if (regimenPensionario is null)
            {
                throw new KeyNotFoundException($"Régimen pensionario {request.Code} no encontrado.");
            }

            regimenPensionario.IsActive = !regimenPensionario.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return regimenPensionario.IsActive;
        }
    }
}