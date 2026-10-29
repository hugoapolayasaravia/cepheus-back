using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.ToggleVendedorStatus
{
    public class ToggleVendedorStatusCommandHandler : IRequestHandler<ToggleVendedorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleVendedorStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleVendedorStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Maestros.Vendedores.Query()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Vendedor {request.Code} no encontrado.");
            }

            entity.IsActive = !entity.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return entity.IsActive;
        }
    }
}
