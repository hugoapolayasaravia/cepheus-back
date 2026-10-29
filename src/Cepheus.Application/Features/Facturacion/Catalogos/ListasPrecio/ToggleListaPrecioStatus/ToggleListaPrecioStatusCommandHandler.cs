using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.ToggleListaPrecioStatus
{
    public class ToggleListaPrecioStatusCommandHandler : IRequestHandler<ToggleListaPrecioStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleListaPrecioStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleListaPrecioStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.ListasPrecio.Query().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Lista de precio {request.Id} no encontrada.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
