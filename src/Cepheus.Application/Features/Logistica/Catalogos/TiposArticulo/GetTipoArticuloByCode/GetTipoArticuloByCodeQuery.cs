using Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.GetTipoArticuloByCode
{
    public record GetTipoArticuloByCodeQuery(string Code) : IRequest<TipoArticuloResponse>;
}