using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.ToggleFormaPagoStatus
{
    public class ToggleFormaPagoStatusCommandHandler : IRequestHandler<ToggleFormaPagoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleFormaPagoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleFormaPagoStatusCommand request, CancellationToken cancellationToken)
        {
            var formaPago = await _uow.Logistica.Catalogos.FormasPago.Query()
                .FirstOrDefaultAsync(f => f.Code == request.Code, cancellationToken);

            if (formaPago is null)
            {
                throw new KeyNotFoundException($"Forma de pago {request.Code} no encontrada.");
            }

            formaPago.IsActive = !formaPago.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return formaPago.IsActive;
        }
    }
}