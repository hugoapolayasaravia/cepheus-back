using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.GetTipoPedidoByCode
{
    public class GetTipoPedidoByCodeQueryHandler : IRequestHandler<GetTipoPedidoByCodeQuery, TipoPedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoPedidoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPedidoResponse> Handle(GetTipoPedidoByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoPedido = await _uow.Logistica.Catalogos.TiposPedido.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoPedidoResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy,
                    UpdatedBy = t.UpdatedBy,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoPedido is null)
            {
                throw new KeyNotFoundException($"Tipo de pedido {request.Code} no encontrado.");
            }

            return tipoPedido;
        }
    }
}