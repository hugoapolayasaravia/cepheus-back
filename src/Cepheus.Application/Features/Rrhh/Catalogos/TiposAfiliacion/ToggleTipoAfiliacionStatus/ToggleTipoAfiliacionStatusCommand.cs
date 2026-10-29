using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.ToggleTipoAfiliacionStatus
{
    public record ToggleTipoAfiliacionStatusCommand(string Code) : IRequest<bool>;
}