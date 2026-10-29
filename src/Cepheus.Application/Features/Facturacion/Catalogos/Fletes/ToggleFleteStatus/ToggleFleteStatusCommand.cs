using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.ToggleFleteStatus
{
    public record ToggleFleteStatusCommand(string Code) : IRequest<bool>;
}
