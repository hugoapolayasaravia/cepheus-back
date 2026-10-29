using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.UpdateActividadMantenimiento
{
    /// <summary>Solo la prioridad es editable; la combinación de actividades es la clave.</summary>
    public record UpdateActividadMantenimientoCommand(
        string ActividadCode,
        string ActividadCubiertaCode,
        int Prioridad,
        byte[] RowVersion
    ) : IRequest<ActividadMantenimientoResponse>;
}
