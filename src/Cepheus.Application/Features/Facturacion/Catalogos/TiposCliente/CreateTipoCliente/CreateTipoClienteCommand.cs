using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.CreateTipoCliente
{
    public record CreateTipoClienteCommand(
        string Name
    ) : IRequest<TipoClienteResponse>;
}
