using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.ToggleOcupacionStatus
{
    public record ToggleOcupacionStatusCommand(string Code) : IRequest<bool>;
}