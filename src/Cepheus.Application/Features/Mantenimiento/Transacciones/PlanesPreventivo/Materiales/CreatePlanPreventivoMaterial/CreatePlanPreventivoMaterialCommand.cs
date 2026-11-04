using Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Materiales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Materiales.CreatePlanPreventivoMaterial
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
