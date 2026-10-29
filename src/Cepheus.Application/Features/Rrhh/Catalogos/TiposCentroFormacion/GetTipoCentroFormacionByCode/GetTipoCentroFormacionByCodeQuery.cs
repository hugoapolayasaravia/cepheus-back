using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.GetTipoCentroFormacionByCode
{
    public record GetTipoCentroFormacionByCodeQuery(string Code) : IRequest<TipoCentroFormacionResponse>;
}