using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.UpdateFormaPagoCancelacion
{
    public record UpdateFormaPagoCancelacionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<FormaPagoCancelacionResponse>;
}
