using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.ToggleModoPagoStatus
{
    public class ToggleModoPagoStatusCommandHandler : IRequestHandler<ToggleModoPagoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleModoPagoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleModoPagoStatusCommand request, CancellationToken cancellationToken)
        {
            var modoPago = await _uow.Rrhh.Catalogos.ModosPago.Query()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (modoPago is null)
            {
                throw new KeyNotFoundException($"Modo de pago {request.Code} no encontrado.");
            }

            modoPago.IsActive = !modoPago.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return modoPago.IsActive;
        }
    }
}