using Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.ManoObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.ManoObra.CreatePlanPreventivoManoObra
{
    public record CreatePlanPreventivoManoObraCommand(
        string PlantaCode,
        string PlanCode,
        string TrabajadorCode,
        decimal Tiempo,
        decimal CostoTotal
    ) : IRequest<PlanPreventivoManoObraResponse>;
}
