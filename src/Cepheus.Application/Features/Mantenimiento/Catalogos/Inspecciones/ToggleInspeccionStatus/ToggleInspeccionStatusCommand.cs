using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.ToggleInspeccionStatus
{
    public record ToggleInspeccionStatusCommand(string Code) : IRequest<bool>;
}
