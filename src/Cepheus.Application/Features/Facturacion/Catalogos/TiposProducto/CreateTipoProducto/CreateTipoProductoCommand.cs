using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.CreateTipoProducto
{
    public record CreateTipoProductoCommand(
        string Code,
        string Name
    ) : IRequest<TipoProductoResponse>;
}
