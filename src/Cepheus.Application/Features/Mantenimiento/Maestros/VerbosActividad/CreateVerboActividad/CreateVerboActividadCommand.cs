using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.CreateVerboActividad
{
    public record CreateVerboActividadCommand(
        string Code,
        string Name
    ) : IRequest<VerboActividadResponse>;
}
