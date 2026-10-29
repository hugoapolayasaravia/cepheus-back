using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.UpdatePlanPreventivo
{
    /// <summary>La clave (planta, código) no se edita.</summary>
    public record UpdatePlanPreventivoCommand(
        string PlantaCode,
        string Code,
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
        decimal TiempoRestante,
        byte[] RowVersion
    ) : IRequest<PlanPreventivoResponse>;
}
