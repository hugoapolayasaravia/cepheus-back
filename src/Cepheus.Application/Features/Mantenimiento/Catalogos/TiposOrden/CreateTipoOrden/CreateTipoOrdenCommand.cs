using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.CreateTipoOrden
{
    public record CreateTipoOrdenCommand(
        string Code,
        string Name
    ) : IRequest<TipoOrdenResponse>;
}
