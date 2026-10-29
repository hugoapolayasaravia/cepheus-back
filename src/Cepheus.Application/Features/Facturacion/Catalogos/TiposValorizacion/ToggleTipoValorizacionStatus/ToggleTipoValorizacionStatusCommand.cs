using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.ToggleTipoValorizacionStatus
{
    public record ToggleTipoValorizacionStatusCommand(string Code) : IRequest<bool>;
}
