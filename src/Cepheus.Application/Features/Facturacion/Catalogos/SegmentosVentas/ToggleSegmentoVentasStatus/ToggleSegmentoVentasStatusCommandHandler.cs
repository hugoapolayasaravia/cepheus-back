using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.ToggleSegmentoVentasStatus
{
    public class ToggleSegmentoVentasStatusCommandHandler : IRequestHandler<ToggleSegmentoVentasStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleSegmentoVentasStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleSegmentoVentasStatusCommand request, CancellationToken cancellationToken)
        {
            var segmentoVentas = await _uow.Facturacion.Catalogos.SegmentosVentas.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (segmentoVentas is null)
            {
                throw new KeyNotFoundException($"Segmento de ventas {request.Code} no encontrado.");
            }

            segmentoVentas.IsActive = !segmentoVentas.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return segmentoVentas.IsActive;
        }
    }
}
