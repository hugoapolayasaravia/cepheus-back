using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.CreateSubCentroEjecutor
{
    public record CreateSubCentroEjecutorCommand(
        string Name,
        string CentroEjecutorCode
    ) : IRequest<SubCentroEjecutorResponse>;
}
