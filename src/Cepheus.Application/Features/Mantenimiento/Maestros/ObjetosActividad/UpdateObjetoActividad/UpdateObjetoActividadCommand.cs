using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.UpdateObjetoActividad
{
    public record UpdateObjetoActividadCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<ObjetoActividadResponse>;
}
