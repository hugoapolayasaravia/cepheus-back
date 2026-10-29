using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.UpdateSubCentroEjecutor
{
    public record UpdateSubCentroEjecutorCommand(
        string Code,
        string Name,
        string CentroEjecutorCode,
        byte[] RowVersion
    ) : IRequest<SubCentroEjecutorResponse>;
}
