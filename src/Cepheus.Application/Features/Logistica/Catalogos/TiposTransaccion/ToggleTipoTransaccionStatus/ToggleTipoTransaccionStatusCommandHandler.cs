using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.ToggleTipoTransaccionStatus
{
    public class ToggleTipoTransaccionStatusCommandHandler : IRequestHandler<ToggleTipoTransaccionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoTransaccionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoTransaccionStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoTransaccion = await _uow.Logistica.Catalogos.TiposTransaccion.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoTransaccion is null)
            {
                throw new KeyNotFoundException($"Tipo de transacción {request.Code} no encontrado.");
            }

            tipoTransaccion.IsActive = !tipoTransaccion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoTransaccion.IsActive;
        }
    }
}
