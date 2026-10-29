using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.ToggleTipoBienStatus
{
    public record ToggleTipoBienStatusCommand(string Code) : IRequest<bool>;
}
