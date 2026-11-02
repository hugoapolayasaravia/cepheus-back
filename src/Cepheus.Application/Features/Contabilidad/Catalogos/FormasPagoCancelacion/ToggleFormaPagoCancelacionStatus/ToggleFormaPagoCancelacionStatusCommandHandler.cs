using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.ToggleFormaPagoCancelacionStatus
{
    public class ToggleFormaPagoCancelacionStatusCommandHandler : IRequestHandler<ToggleFormaPagoCancelacionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleFormaPagoCancelacionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleFormaPagoCancelacionStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Catalogos.FormasPagoCancelacion.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Forma de pago de cancelación {request.Code} no encontrada.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
