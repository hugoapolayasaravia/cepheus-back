using System.Linq.Expressions;
using Cepheus.Domain.Mantenimiento.Transacciones;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common
{
    /// <summary>Proyección única entidad -> respuesta (traducible por EF).</summary>
    public static class UnidadHorometroProjection
    {
        public static readonly Expression<Func<UnidadHorometro, UnidadHorometroResponse>> ToResponse = x => new UnidadHorometroResponse
        {
            SubCentroCostoCode = x.SubCentroCostoCode,
            SubCentroCostoName = x.SubCentroCosto.Name,
            Fecha = x.Fecha,
            Horometro = x.Horometro,
            Estado = x.Estado,
            PlantaAfectadaCode = x.PlantaAfectadaCode,
            PlantaAfectadaName = x.PlantaAfectada.Name,
            EquipoCode = x.EquipoCode,
            EquipoName = x.Equipo == null ? null : x.Equipo.Name,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            RowVersion = x.RowVersion
        };
    }
}
