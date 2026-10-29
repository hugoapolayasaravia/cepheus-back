using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.CreateCentroEjecutor
{
    public record CreateCentroEjecutorCommand(
        string Code,
        string Name
    ) : IRequest<CentroEjecutorResponse>;
}
