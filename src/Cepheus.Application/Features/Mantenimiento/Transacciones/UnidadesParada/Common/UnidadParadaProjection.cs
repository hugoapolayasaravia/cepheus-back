using System.Linq.Expressions;
using Cepheus.Domain.Mantenimiento.Transacciones;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common
{
    /// <summary>Proyección única entidad -> respuesta (traducible por EF).</summary>
    public static class UnidadParadaProjection
    {
        public static readonly Expression<Func<UnidadParada, UnidadParadaResponse>> ToResponse = x => new UnidadParadaResponse
        {
            SubCentroCostoCode = x.SubCentroCostoCode,
            SubCentroCostoName = x.SubCentroCosto.Name,
            Fecha = x.Fecha,
            Horas = x.Horas,
            Estado = x.Estado,
            PlantaAfectadaCode = x.PlantaAfectadaCode,
            PlantaAfectadaName = x.PlantaAfectada.Name,
            FallaCode = x.FallaCode,
            FallaName = x.Falla == null ? null : x.Falla.Name,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            RowVersion = x.RowVersion
        };
    }
}
