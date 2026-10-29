using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.GetTipoContratoByCode
{
    public record GetTipoContratoByCodeQuery(string Code) : IRequest<TipoContratoResponse>;
}