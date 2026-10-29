using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.UpdateTipoOperacion
{
    public record UpdateTipoOperacionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoOperacionResponse>;
}
