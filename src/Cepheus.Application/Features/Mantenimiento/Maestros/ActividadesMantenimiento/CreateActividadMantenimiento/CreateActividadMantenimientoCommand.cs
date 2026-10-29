using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.CreateActividadMantenimiento
{
    public record CreateActividadMantenimientoCommand(
        string ActividadCode,
        string ActividadCubiertaCode,
        int Prioridad
    ) : IRequest<ActividadMantenimientoResponse>;
}
