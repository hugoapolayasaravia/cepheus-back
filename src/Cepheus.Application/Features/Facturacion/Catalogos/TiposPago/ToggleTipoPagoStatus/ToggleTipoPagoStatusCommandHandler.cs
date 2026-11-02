using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.ToggleTipoPagoStatus
{
    public class ToggleTipoPagoStatusCommandHandler : IRequestHandler<ToggleTipoPagoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoPagoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoPagoStatusCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var tipo = await _uow.Facturacion.Catalogos.TiposPago.Query()
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException($"Tipo de pago {request.Code} no encontrado.");

            tipo.IsActive = !tipo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipo.IsActive;
        }
    }
}
