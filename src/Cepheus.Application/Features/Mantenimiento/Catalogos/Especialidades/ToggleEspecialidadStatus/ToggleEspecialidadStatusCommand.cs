using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.ToggleEspecialidadStatus
{
    public record ToggleEspecialidadStatusCommand(string Code) : IRequest<bool>;
}
