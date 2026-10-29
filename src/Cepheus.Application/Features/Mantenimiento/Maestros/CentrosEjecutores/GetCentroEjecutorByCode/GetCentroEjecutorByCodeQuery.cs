using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.GetCentroEjecutorByCode
{
    public record GetCentroEjecutorByCodeQuery(string Code) : IRequest<CentroEjecutorResponse>;
}
