using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.ToggleNotaCompraStatus
{
    public record ToggleNotaCompraStatusCommand(string Code) : IRequest<bool>;
}