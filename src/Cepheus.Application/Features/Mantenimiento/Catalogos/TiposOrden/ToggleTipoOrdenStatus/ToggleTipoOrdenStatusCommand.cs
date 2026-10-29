using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.ToggleTipoOrdenStatus
{
    public record ToggleTipoOrdenStatusCommand(string Code) : IRequest<bool>;
}
