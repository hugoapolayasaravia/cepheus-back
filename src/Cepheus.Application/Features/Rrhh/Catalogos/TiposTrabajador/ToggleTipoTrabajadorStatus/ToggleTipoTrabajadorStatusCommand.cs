using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.ToggleTipoTrabajadorStatus
{
    public record ToggleTipoTrabajadorStatusCommand(string Code) : IRequest<bool>;
}