using Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.ManoObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.ManoObra.UpdatePlanPreventivoManoObra
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
