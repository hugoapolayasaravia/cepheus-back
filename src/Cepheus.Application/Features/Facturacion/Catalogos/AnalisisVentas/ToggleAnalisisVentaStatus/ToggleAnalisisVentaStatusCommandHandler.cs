using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.ToggleAnalisisVentaStatus
{
    public class ToggleAnalisisVentaStatusCommandHandler : IRequestHandler<ToggleAnalisisVentaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleAnalisisVentaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleAnalisisVentaStatusCommand request, CancellationToken cancellationToken)
        {
            var analisisVenta = await _uow.Facturacion.Catalogos.AnalisisVentas.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (analisisVenta is null)
            {
                throw new KeyNotFoundException($"Análisis de ventas {request.Code} no encontrado.");
            }

            analisisVenta.IsActive = !analisisVenta.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return analisisVenta.IsActive;
        }
    }
}
