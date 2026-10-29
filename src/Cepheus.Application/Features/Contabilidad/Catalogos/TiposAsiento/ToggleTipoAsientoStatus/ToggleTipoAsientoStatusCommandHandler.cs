using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.ToggleTipoAsientoStatus
{
    public class ToggleTipoAsientoStatusCommandHandler : IRequestHandler<ToggleTipoAsientoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoAsientoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoAsientoStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Catalogos.TiposAsiento.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de asiento {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
