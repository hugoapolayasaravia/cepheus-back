using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.GetFormaPagoByCode
{
    public record GetFormaPagoByCodeQuery(string Code) : IRequest<FormaPagoResponse>;
}