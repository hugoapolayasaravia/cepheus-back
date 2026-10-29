using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.ToggleLugarEnvioStatus
{
    public record ToggleLugarEnvioStatusCommand(string Code) : IRequest<bool>;
}