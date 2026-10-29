using System.Linq.Expressions;
using Cepheus.Domain.Mantenimiento.Maestros;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common
{
    /// <summary>Proyección única entidad -> respuesta (traducible por EF).</summary>
    public static class PlanPreventivoProjection
    {
        public static readonly Expression<Func<PlanMantenimientoPreventivo, PlanPreventivoResponse>> ToResponse = x => new PlanPreventivoResponse
        {
            PlantaCode = x.PlantaCode,
            Code = x.Code,
            Description = x.Description,
            EquipoCode = x.EquipoCode,
            EquipoName = x.Equipo.Name,
            EspecialidadCode = x.EspecialidadCode,
            EspecialidadName = x.Especialidad.Name,
            ResponsableCode = x.ResponsableCode,
            ResponsableName = (x.Responsable.FirstNames + " " + x.Responsable.PaternalSurname + " " + x.Responsable.MaternalSurname).Trim(),
            SubCentroCostoCode = x.SubCentroCostoCode,
            SubCentroCostoName = x.SubCentroCosto.Name,
            SubCentroEjecutorCode = x.SubCentroEjecutorCode,
            SubCentroEjecutorName = x.SubCentroEjecutor.Name,
            ActividadCode = x.ActividadCode,
            ActividadName = x.Actividad.VerboActividad.Name + " " + x.Actividad.ObjetoActividad.Name,
            Frecuencia = x.Frecuencia,
            UnidadFrecuencia = x.UnidadFrecuencia,
            Factor = x.Factor,
            FechaInicio = x.FechaInicio,
            FechaTermino = x.FechaTermino,
            FrecuenciaGama = x.FrecuenciaGama,
            TiempoRestante = x.TiempoRestante,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            RowVersion = x.RowVersion
        };
    }
}
