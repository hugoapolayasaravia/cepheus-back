using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.UpdatePlanPreventivoManoObra
{
    public record UpdatePlanPreventivoManoObraCommand(
        string PlantaCode,
        string PlanCode,
        string TrabajadorCode,
        decimal Tiempo,
        decimal CostoTotal,
        byte[] RowVersion
    ) : IRequest<PlanPreventivoManoObraResponse>;
}
