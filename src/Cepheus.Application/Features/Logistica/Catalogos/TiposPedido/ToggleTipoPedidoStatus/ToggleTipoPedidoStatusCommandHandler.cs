using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.ToggleTipoPedidoStatus
{
    public class ToggleTipoPedidoStatusCommandHandler : IRequestHandler<ToggleTipoPedidoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleTipoPedidoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleTipoPedidoStatusCommand request, CancellationToken cancellationToken)
        {
            var tipoPedido = await _uow.Logistica.Catalogos.TiposPedido.Query()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (tipoPedido is null)
            {
                throw new KeyNotFoundException($"Tipo de pedido {request.Code} no encontrado.");
            }

            tipoPedido.IsActive = !tipoPedido.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return tipoPedido.IsActive;
        }
    }
}