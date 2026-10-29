using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.ToggleClasificacionClienteStatus
{
    public record ToggleClasificacionClienteStatusCommand(string Code) : IRequest<bool>;
}
