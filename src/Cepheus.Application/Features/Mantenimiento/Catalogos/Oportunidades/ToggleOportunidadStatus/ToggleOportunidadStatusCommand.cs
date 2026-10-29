using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.ToggleOportunidadStatus
{
    public record ToggleOportunidadStatusCommand(string Code) : IRequest<bool>;
}
