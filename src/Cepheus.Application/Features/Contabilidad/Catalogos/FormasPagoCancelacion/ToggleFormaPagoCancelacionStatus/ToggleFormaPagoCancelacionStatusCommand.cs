using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.ToggleFormaPagoCancelacionStatus
{
    public record ToggleFormaPagoCancelacionStatusCommand(string Code) : IRequest<bool>;
}
