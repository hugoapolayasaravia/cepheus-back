using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.ToggleCategoriaProductoStatus
{
    public class ToggleCategoriaProductoStatusCommandHandler : IRequestHandler<ToggleCategoriaProductoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleCategoriaProductoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleCategoriaProductoStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.CategoriasProducto.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Categoría de producto {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
