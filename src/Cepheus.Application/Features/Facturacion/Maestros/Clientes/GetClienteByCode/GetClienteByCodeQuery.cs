using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.GetClienteByCode
{
    public record GetClienteByCodeQuery(string Code) : IRequest<ClienteResponse>;
}
