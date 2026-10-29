using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.CreatePlanPreventivoManoObra
{
    public record CreatePlanPreventivoManoObraCommand(
        string PlantaCode,
        string PlanCode,
        string TrabajadorCode,
        decimal Tiempo,
        decimal CostoTotal
    ) : IRequest<PlanPreventivoManoObraResponse>;
}
