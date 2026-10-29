using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.CreateActividad
{
    /// <summary>
    /// El Code NO se recibe: se calcula en el handler como
    /// VerboActividadCode + ObjetoActividadCode.
    /// </summary>
    public record CreateActividadCommand(
        string VerboActividadCode,
        string ObjetoActividadCode
    ) : IRequest<ActividadResponse>;
}
