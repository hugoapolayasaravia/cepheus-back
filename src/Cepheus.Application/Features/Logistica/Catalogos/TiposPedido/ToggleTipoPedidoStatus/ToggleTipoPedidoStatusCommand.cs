using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.ToggleTipoPedidoStatus
{
    public record ToggleTipoPedidoStatusCommand(string Code) : IRequest<bool>;
}