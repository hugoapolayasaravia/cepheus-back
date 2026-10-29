using MediatR;

namespace Cepheus.Application.Features.Comunes.Plantas.TogglePlantaStatus
{
    public record TogglePlantaStatusCommand(string Code) : IRequest<bool>;
}
