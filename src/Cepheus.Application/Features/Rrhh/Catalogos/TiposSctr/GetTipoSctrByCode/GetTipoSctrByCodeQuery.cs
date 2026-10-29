using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.GetTipoSctrByCode
{
    public record GetTipoSctrByCodeQuery(string Code) : IRequest<TipoSctrResponse>;
}