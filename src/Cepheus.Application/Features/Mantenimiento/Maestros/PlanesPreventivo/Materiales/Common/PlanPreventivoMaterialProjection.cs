using System.Linq.Expressions;
using Cepheus.Domain.Mantenimiento.Maestros;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.Common
{
    public static class PlanPreventivoMaterialProjection
    {
        public static readonly Expression<Func<PlanMantenimientoPreventivoMaterial, PlanPreventivoMaterialResponse>> ToResponse = x => new PlanPreventivoMaterialResponse
        {
            PlantaCode = x.PlantaCode,
            PlanCode = x.PlanCode,
            ArticuloCode = x.ArticuloCode,
            ArticuloName = x.Articulo.Name,
            Cantidad = x.Cantidad,
            CostoUnitario = x.CostoUnitario,
            CostoTotal = x.CostoTotal,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            RowVersion = x.RowVersion
        };
    }
}
