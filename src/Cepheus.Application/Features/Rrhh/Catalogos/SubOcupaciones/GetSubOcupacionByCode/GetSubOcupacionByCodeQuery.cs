using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.GetSubOcupacionByCode
{
    public record GetSubOcupacionByCodeQuery(string Code) : IRequest<SubOcupacionResponse>;
}