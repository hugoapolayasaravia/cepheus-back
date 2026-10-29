using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.ToggleEspecialidadStatus
{
    public record ToggleEspecialidadStatusCommand(string Code) : IRequest<bool>;
}