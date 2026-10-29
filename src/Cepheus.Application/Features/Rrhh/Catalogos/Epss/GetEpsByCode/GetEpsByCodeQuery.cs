using Cepheus.Application.Features.Rrhh.Catalogos.Epss.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.GetEpsByCode
{
    public record GetEpsByCodeQuery(string Code) : IRequest<EpsResponse>;
}