using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.UpdateTipoValorizacion
{
    public record UpdateTipoValorizacionCommand(
        string Code,
        string Name,
        int Days,
        byte[] RowVersion
    ) : IRequest<TipoValorizacionResponse>;
}
