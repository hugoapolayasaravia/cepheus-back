using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.ToggleNotaCompraStatus
{
    public class ToggleNotaCompraStatusCommandHandler : IRequestHandler<ToggleNotaCompraStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleNotaCompraStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleNotaCompraStatusCommand request, CancellationToken cancellationToken)
        {
            var nota = await _uow.Logistica.Catalogos.NotasCompra.Query()
                .FirstOrDefaultAsync(n => n.Code == request.Code, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException($"Nota de compra {request.Code} no encontrada.");
            }

            nota.IsActive = !nota.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return nota.IsActive;
        }
    }
}