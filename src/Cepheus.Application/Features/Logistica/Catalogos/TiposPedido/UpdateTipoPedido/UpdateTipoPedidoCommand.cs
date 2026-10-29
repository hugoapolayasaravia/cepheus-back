using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.UpdateTipoPedido
{
    public record UpdateTipoPedidoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoPedidoResponse>;
}