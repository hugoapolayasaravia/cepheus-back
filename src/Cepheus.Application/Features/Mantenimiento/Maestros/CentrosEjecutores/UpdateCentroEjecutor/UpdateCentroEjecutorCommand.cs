using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.UpdateCentroEjecutor
{
    public record UpdateCentroEjecutorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<CentroEjecutorResponse>;
}
