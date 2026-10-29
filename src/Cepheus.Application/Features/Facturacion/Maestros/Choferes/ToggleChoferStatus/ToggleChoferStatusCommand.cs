using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.ToggleChoferStatus
{
    public record ToggleChoferStatusCommand(string TransportistaCode, string Code) : IRequest<bool>;
}
