using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Plantas.TogglePlantaStatus
{
    public class TogglePlantaStatusCommandHandler : IRequestHandler<TogglePlantaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public TogglePlantaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(TogglePlantaStatusCommand request, CancellationToken cancellationToken)
        {
            var planta = await _uow.Comunes.Plantas.GetByCodeAsync(request.Code, cancellationToken);

            if (planta is null)
            {
                throw new KeyNotFoundException($"Planta {request.Code} no encontrada.");
            }

            planta.IsActive = !planta.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return planta.IsActive;
        }
    }
}
