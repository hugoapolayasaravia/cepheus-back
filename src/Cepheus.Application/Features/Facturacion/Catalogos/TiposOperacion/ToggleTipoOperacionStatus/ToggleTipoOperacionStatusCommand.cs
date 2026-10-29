using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.ToggleTipoOperacionStatus
{
    public record ToggleTipoOperacionStatusCommand(string Code) : IRequest<bool>;
}
