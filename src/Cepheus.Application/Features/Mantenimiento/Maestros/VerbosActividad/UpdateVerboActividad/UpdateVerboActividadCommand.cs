using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.UpdateVerboActividad
{
    public record UpdateVerboActividadCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<VerboActividadResponse>;
}
