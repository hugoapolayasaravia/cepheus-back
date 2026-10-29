using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.UpdateTipoOrden
{
    public record UpdateTipoOrdenCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoOrdenResponse>;
}
