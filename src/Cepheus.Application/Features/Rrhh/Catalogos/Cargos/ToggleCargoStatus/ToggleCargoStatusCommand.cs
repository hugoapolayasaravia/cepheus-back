using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.ToggleCargoStatus
{
    public record ToggleCargoStatusCommand(string Code) : IRequest<bool>;
}