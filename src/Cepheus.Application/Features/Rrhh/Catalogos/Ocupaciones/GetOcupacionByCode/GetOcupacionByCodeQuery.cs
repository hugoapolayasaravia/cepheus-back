using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.GetOcupacionByCode
{
    public record GetOcupacionByCodeQuery(string Code) : IRequest<OcupacionResponse>;
}