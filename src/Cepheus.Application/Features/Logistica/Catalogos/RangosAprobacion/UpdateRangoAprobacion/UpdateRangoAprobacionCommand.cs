using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.UpdateRangoAprobacion
{
    /// <summary>Solo permite editar los importes — la clave (Nivel+Transacción+Unidad+Moneda) no se edita.</summary>
    public record UpdateRangoAprobacionCommand(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode,
        decimal ImporteMinimo,
        decimal ImporteMaximo,
        decimal ImporteAcumuladoDiario,
        decimal ImporteAcumuladoMensual,
        decimal? PorcentajeTotal,
        byte[] RowVersion
    ) : IRequest<RangoAprobacionResponse>;
}
