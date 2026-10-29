using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.GetSctrPensionByCode
{
    public record GetSctrPensionByCodeQuery(string Code) : IRequest<SctrPensionResponse>;
}