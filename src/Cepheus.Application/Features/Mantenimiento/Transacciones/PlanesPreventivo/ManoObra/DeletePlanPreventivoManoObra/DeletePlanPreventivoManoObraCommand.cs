using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.ManoObra.DeletePlanPreventivoManoObra
{
    public record DeletePlanPreventivoManoObraCommand(string PlantaCode, string PlanCode, string TrabajadorCode) : IRequest;
}
