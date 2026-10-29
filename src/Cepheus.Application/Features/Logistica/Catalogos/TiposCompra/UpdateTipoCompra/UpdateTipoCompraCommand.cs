using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.UpdateTipoCompra
{
    public record UpdateTipoCompraCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoCompraResponse>;
}