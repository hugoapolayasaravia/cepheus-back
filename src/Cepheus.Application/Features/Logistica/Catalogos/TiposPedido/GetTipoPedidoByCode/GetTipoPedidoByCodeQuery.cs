using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.GetTipoPedidoByCode
{
    public record GetTipoPedidoByCodeQuery(string Code) : IRequest<TipoPedidoResponse>;
}