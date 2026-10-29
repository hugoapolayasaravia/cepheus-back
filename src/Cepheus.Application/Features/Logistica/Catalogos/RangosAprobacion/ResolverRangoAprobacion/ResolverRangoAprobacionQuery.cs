using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.ResolverRangoAprobacion
{
    /// <summary>
    /// Dado un (TipoTransaccion, UnidadNegocio, Moneda, Monto), resuelve qué
    /// RangoAprobacion aplica — subiendo por la jerarquía de UnidadNegocio
    /// (ParentCode) si la unidad exacta no tiene parametrización propia,
    /// confirmado con el usuario a partir del árbol de unidades de negocio.
    /// </summary>
    public record ResolverRangoAprobacionQuery(
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode,
        decimal Monto
    ) : IRequest<ResolverRangoAprobacionResult>;

    public class ResolverRangoAprobacionResult
    {
        /// <summary>Unidad de negocio donde efectivamente se encontró parametrización (puede ser un ancestro de la solicitada).</summary>
        public string UnidadNegocioEfectivaCode { get; set; } = default!;

        /// <summary>Todos los rangos configurados en la unidad efectiva, para esa transacción y moneda (referencia — para ver toda la escala de niveles).</summary>
        public List<RangoAprobacionResponse> RangosDeLaUnidad { get; set; } = new();

        /// <summary>El rango cuyo [ImporteMinimo, ImporteMaximo] contiene el Monto solicitado, si existe.</summary>
        public RangoAprobacionResponse? RangoAplicable { get; set; }
    }
}
