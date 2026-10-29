using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.UpdateTipoCliente
{
    public record UpdateTipoClienteCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoClienteResponse>;
}
