using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.ToggleNivelTrabajadorStatus
{
    public record ToggleNivelTrabajadorStatusCommand(string Code) : IRequest<bool>;
}