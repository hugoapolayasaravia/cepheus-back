using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.CreateTipoPedido
{
    public class CreateTipoPedidoCommandHandler : IRequestHandler<CreateTipoPedidoCommand, TipoPedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoPedidoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPedidoResponse> Handle(CreateTipoPedidoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.TiposPedido.Query().Select(f => f.Code), length: 2, entityLabel: "Tipos de Pedido", cancellationToken);


            var tipoPedido = new TipoPedido
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.TiposPedido.AddAsync(tipoPedido, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoPedido);
        }

        internal static TipoPedidoResponse Map(TipoPedido tipoPedido) => new()
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