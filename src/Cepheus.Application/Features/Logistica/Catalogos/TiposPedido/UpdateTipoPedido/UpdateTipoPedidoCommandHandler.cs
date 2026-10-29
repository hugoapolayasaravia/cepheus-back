using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.UpdateTipoPedido
{
    public class UpdateTipoPedidoCommandHandler : IRequestHandler<UpdateTipoPedidoCommand, TipoPedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoPedidoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPedidoResponse> Handle(UpdateTipoPedidoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.TiposPedido.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de pedido {request.Code} no encontrado.");
            }

            var tipoPedido = new TipoPedido
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.TiposPedido.Update(tipoPedido);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de pedido fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoPedidoResponse
            {
                Code = tipoPedido.Code,
                Name = tipoPedido.Name,
                IsActive = tipoPedido.IsActive,
                CreatedAt = tipoPedido.CreatedAt,
                UpdatedAt = tipoPedido.UpdatedAt,
                CreatedBy = tipoPedido.CreatedBy,
                UpdatedBy = tipoPedido.UpdatedBy,
                RowVersion = tipoPedido.RowVersion
            };
        }
    }
}