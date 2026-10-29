using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.GetSituacionEpsByCode
{
    public record GetSituacionEpsByCodeQuery(string Code) : IRequest<SituacionEpsResponse>;
}