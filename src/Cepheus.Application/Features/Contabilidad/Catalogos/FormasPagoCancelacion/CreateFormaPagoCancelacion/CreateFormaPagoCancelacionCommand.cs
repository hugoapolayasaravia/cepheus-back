using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.CreateFormaPagoCancelacion
{
    public record CreateFormaPagoCancelacionCommand(
        string Code,
        string Name
    ) : IRequest<FormaPagoCancelacionResponse>;
}
