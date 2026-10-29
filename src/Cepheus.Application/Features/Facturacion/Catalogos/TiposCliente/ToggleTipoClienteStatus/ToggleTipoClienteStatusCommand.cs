using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.ToggleTipoClienteStatus
{
    public record ToggleTipoClienteStatusCommand(string Code) : IRequest<bool>;
}
