using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.ToggleTipoCompraStatus
{
    public class ToggleTipoCompraStatusCommandHandler : IRequestHandler<ToggleTipoCompraStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoCompraStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoCompraStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoCompra = await _uow.Logistica.Catalogos.TiposCompra.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoCompra is null)
            {
                throw new KeyNotFoundException($"Tipo de compra {request.Code} no encontrado.");
            }

            tipoCompra.IsActive = !tipoCompra.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoCompra.IsActive;
        }
    }
}