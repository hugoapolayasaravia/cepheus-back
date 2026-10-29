using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.UpdateTipoProducto
{
    public record UpdateTipoProductoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoProductoResponse>;
}
