using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.ToggleSctrPensionStatus
{
    public record ToggleSctrPensionStatusCommand(string Code) : IRequest<bool>;
}