using System.Linq.Expressions;
using Cepheus.Domain.Mantenimiento.Maestros;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common
{
    public static class ActividadMantenimientoProjection
    {
        public static readonly Expression<Func<ActividadMantenimiento, ActividadMantenimientoResponse>> ToResponse =
            x => new ActividadMantenimientoResponse
            {
                ActividadCode = x.ActividadCode,
                ActividadName = x.Actividad.VerboActividad.Name + " " + x.Actividad.ObjetoActividad.Name,
                ActividadCubiertaCode = x.ActividadCubiertaCode,
                ActividadCubiertaName = x.ActividadCubierta.VerboActividad.Name + " " + x.ActividadCubierta.ObjetoActividad.Name,
                Prioridad = x.Prioridad,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,

                RowVersion = x.RowVersion
            };
    }
}
