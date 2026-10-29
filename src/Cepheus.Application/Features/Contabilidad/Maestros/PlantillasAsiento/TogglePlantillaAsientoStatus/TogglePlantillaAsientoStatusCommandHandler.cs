using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.TogglePlantillaAsientoStatus
{
    public class TogglePlantillaAsientoStatusCommandHandler : IRequestHandler<TogglePlantillaAsientoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public TogglePlantillaAsientoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(TogglePlantillaAsientoStatusCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var entity = await _uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Plantilla de asiento {code} no encontrada.");
            }

            if (!entity.IsActive)
            {
                await PlantillaAsientoRules.EnsureSingleActiveAsync(_uow, entity.Process, code, cancellationToken);
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
