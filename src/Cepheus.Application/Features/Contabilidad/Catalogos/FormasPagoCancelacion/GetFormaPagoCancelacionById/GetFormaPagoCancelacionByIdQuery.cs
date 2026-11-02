using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.GetFormaPagoCancelacionById
{
    public record GetFormaPagoCancelacionByIdQuery(string Code) : IRequest<FormaPagoCancelacionResponse>;
}
