using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.CopyPlanPreventivo
{
    /// <summary>
    /// Copia un plan preventivo con su mano de obra y materiales, bajo un código nuevo de la misma planta
    /// (legacy Logi_sp_Actualiza_ManPreventivo). La copia queda sin fechas de inicio ni término.
    /// </summary>
    public record CopyPlanPreventivoCommand(string PlantaCode, string Code) : IRequest<PlanPreventivoResponse>;
}
