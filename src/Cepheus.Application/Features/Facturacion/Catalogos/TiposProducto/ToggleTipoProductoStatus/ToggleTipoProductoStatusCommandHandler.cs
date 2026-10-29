using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.ToggleTipoProductoStatus
{
    public class ToggleTipoProductoStatusCommandHandler : IRequestHandler<ToggleTipoProductoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoProductoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoProductoStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.TiposProducto.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de producto {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
