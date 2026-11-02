using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.GetTipoPagoByCode
{
    public record GetTipoPagoByCodeQuery(string Code) : IRequest<TipoPagoResponse>;
}
