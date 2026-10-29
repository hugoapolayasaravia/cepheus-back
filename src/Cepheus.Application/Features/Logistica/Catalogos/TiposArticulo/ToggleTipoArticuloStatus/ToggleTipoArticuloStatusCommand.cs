using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.ToggleTipoArticuloStatus
{
    public record ToggleTipoArticuloStatusCommand(string Code) : IRequest<bool>;
}