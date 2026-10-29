using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.GetSctrSaludByCode
{
    public record GetSctrSaludByCodeQuery(string Code) : IRequest<SctrSaludResponse>;
}