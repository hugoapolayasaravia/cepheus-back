using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.ToggleCompradorStatus
{
    public record ToggleCompradorStatusCommand(string Code) : IRequest<bool>;
}