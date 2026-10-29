using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.ToggleFormaPagoVentaStatus
{
    public class ToggleFormaPagoVentaStatusCommandHandler : IRequestHandler<ToggleFormaPagoVentaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleFormaPagoVentaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleFormaPagoVentaStatusCommand request, CancellationToken cancellationToken)
        {
            var formaPagoVenta = await _uow.Facturacion.Catalogos.FormasPagoVenta.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (formaPagoVenta is null)
            {
                throw new KeyNotFoundException($"Forma de pago de ventas {request.Code} no encontrada.");
            }

            formaPagoVenta.IsActive = !formaPagoVenta.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return formaPagoVenta.IsActive;
        }
    }
}
