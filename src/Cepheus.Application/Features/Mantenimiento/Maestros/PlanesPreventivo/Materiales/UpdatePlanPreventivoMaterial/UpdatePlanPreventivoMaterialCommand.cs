using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.UpdatePlanPreventivoMaterial
{
    public record UpdatePlanPreventivoMaterialCommand(
        string PlantaCode,
        string PlanCode,
        string ArticuloCode,
        decimal Cantidad,
        decimal CostoUnitario,
        decimal CostoTotal,
        byte[] RowVersion
    ) : IRequest<PlanPreventivoMaterialResponse>;
}
