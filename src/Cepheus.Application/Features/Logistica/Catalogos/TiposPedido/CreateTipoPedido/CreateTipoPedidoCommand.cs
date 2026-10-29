using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.CreateTipoPedido
{
    public record CreateTipoPedidoCommand(
        string Name
    ) : IRequest<TipoPedidoResponse>;
}