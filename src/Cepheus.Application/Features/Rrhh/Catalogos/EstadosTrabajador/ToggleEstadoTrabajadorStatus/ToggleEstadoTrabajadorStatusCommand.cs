using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.ToggleEstadoTrabajadorStatus
{
    public record ToggleEstadoTrabajadorStatusCommand(string Code) : IRequest<bool>;
}