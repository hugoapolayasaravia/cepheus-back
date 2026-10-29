using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.CreateObjetoActividad
{
    public record CreateObjetoActividadCommand(
        string Code,
        string Name
    ) : IRequest<ObjetoActividadResponse>;
}
