using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.ToggleTipoCompraStatus
{
    public record ToggleTipoCompraStatusCommand(string Code) : IRequest<bool>;
}