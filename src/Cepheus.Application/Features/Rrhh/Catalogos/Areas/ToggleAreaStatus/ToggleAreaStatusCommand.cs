using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.ToggleAreaStatus
{
    public record ToggleAreaStatusCommand(string Code) : IRequest<bool>;
}