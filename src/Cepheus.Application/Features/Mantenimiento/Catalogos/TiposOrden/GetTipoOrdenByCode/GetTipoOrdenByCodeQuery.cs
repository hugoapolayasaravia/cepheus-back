using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.GetTipoOrdenByCode
{
    public record GetTipoOrdenByCodeQuery(string Code) : IRequest<TipoOrdenResponse>;
}
