using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.ToggleCobradorStatus
{
    public class ToggleCobradorStatusCommandHandler : IRequestHandler<ToggleCobradorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCobradorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCobradorStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Maestros.Cobradores.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Cobrador {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
