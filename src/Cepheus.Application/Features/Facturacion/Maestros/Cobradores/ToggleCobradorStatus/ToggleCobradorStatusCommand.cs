using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.ToggleCobradorStatus
{
    public record ToggleCobradorStatusCommand(string Code) : IRequest<bool>;
}
