using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.CreatePlanPreventivo
{
    /// <summary>El código ('MP' + correlativo de 4 dígitos por planta) lo genera el backend.</summary>
    public record CreatePlanPreventivoCommand(
        string PlantaCode,
        string Description,
        string EquipoCode,
        string EspecialidadCode,
        string ResponsableCode,
        string SubCentroCostoCode,
        string SubCentroEjecutorCode,
        string ActividadCode,
        decimal Frecuencia,
        UnidadFrecuencia UnidadFrecuencia,
        decimal Factor,
        DateTime? FechaInicio,
        DateTime? FechaTermino,
        decimal FrecuenciaGama,
        decimal TiempoRestante
    ) : IRequest<PlanPreventivoResponse>;
}
