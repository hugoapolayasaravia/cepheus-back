using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.ToggleAlturaLosaStatus
{
    public record ToggleAlturaLosaStatusCommand(string Code) : IRequest<bool>;
}
