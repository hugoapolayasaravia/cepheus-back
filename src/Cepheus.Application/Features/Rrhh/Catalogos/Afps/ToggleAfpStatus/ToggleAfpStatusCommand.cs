using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.ToggleAfpStatus
{
    public record ToggleAfpStatusCommand(string Code) : IRequest<bool>;
}