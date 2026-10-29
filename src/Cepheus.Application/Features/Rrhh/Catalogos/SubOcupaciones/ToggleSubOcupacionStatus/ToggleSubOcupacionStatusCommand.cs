using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.ToggleSubOcupacionStatus
{
    public record ToggleSubOcupacionStatusCommand(string Code) : IRequest<bool>;
}