using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.GetTipoViaByCode
{
    public record GetTipoViaByCodeQuery(string Code) : IRequest<TipoViaResponse>;
}