using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.GetTipoCompraByCode
{
    public record GetTipoCompraByCodeQuery(string Code) : IRequest<TipoCompraResponse>;
}