using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.ToggleUnidadMedidaVentaStatus
{
    public class ToggleUnidadMedidaVentaStatusCommandHandler : IRequestHandler<ToggleUnidadMedidaVentaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleUnidadMedidaVentaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleUnidadMedidaVentaStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Unidad de medida de venta {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
