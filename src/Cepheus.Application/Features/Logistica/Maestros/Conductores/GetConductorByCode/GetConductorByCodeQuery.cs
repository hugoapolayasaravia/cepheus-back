using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductorByCode
{
    public record GetConductorByCodeQuery(string Code) : IRequest<ConductorResponse>;
}
