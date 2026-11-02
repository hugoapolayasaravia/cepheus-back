using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.ToggleNoAfectoRetencionStatus
{
    public class ToggleNoAfectoRetencionStatusCommandHandler : IRequestHandler<ToggleNoAfectoRetencionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNoAfectoRetencionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNoAfectoRetencionStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.NoAfectosRetencion.Query()
                .FirstOrDefaultAsync(x => x.Ruc == request.Ruc && x.Type == request.Type, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No afecto a retención {request.Ruc}/{request.Type} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
