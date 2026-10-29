using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.CreatePlanPreventivoMaterial
{
    public record CreatePlanPreventivoMaterialCommand(
        string PlantaCode,
        string PlanCode,
        string ArticuloCode,
        decimal Cantidad,
        decimal CostoUnitario,
        decimal CostoTotal
    ) : IRequest<PlanPreventivoMaterialResponse>;
}
