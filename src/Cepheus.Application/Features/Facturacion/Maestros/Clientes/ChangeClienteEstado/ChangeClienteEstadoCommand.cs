using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.ChangeClienteEstado
{
    public record ChangeClienteEstadoCommand(
        string Code,
        string NuevoEstado
    ) : IRequest<ClienteResponse>;
}
