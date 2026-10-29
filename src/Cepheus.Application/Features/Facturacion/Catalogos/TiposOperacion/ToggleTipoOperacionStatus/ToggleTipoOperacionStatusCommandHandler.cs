using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.ToggleTipoOperacionStatus
{
    public class ToggleTipoOperacionStatusCommandHandler : IRequestHandler<ToggleTipoOperacionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoOperacionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoOperacionStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.TiposOperacion.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de operación {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
