using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.ToggleComprobantePagoStatus
{
    public class ToggleComprobantePagoStatusCommandHandler : IRequestHandler<ToggleComprobantePagoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleComprobantePagoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleComprobantePagoStatusCommand request, CancellationToken cancellationToken)
        {
            var comprobante = await _uow.Comunes.ComprobantesPago.GetByCodeAsync(request.Code, cancellationToken);

            if (comprobante is null)
            {
                throw new KeyNotFoundException($"Comprobante de pago {request.Code} no encontrado.");
            }

            comprobante.IsActive = !comprobante.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return comprobante.IsActive;
        }
    }
}

