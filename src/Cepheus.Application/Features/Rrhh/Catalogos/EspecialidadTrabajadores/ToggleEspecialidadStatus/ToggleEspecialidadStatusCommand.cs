using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.ToggleEspecialidadStatus
{
    public record ToggleEspecialidadStatusCommand(string Code) : IRequest<bool>;
}