using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.ToggleFallaStatus
{
    public record ToggleFallaStatusCommand(string Code) : IRequest<bool>;
}
