using System.Linq.Expressions;
using Cepheus.Domain.Mantenimiento.Maestros;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common
{
    public static class PlanPreventivoManoObraProjection
    {
        public static readonly Expression<Func<PlanMantenimientoPreventivoManoObra, PlanPreventivoManoObraResponse>> ToResponse = x => new PlanPreventivoManoObraResponse
        {
            PlantaCode = x.PlantaCode,
            PlanCode = x.PlanCode,
            TrabajadorCode = x.TrabajadorCode,
            TrabajadorName = (x.Trabajador.FirstNames + " " + x.Trabajador.PaternalSurname + " " + x.Trabajador.MaternalSurname).Trim(),
            Tiempo = x.Tiempo,
            CostoTotal = x.CostoTotal,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            RowVersion = x.RowVersion
        };
    }
}
