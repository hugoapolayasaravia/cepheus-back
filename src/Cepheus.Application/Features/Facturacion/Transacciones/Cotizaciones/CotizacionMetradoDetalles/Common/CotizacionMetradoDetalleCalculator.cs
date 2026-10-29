using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common
{
    /// <summary>
    /// Fórmulas de metrado confirmadas contra el código fuente original de la
    /// ventana w_cotizaciones (eventos ue_actualizavalores / ue_actualizavalores1).
    /// Cubre SOLO las fórmulas que no dependen de las funciones SQL
    /// dbo.Apoyo_Vigueta / dbo.Apoyo_Vigueta_v2 / dbo.Filas, cuyo cuerpo no
    /// se ha confirmado todavía: Support (Apoyo_Cot) y, en el sistema V1,
    /// Row (Fila_Cot) siguen siendo capturados manualmente por quien llama
    /// (no se recalculan acá) hasta poder portar esas 3 funciones.
    ///
    /// Aplica después de fijar los campos de entrada de la línea
    /// (InnerLength, Times, Anchorage, Support, Row, Width, Spacing,
    /// Widening, WastePercentage, Support2, PolystyrenePrice) y antes de
    /// persistir.
    /// </summary>
    internal static class CotizacionMetradoDetalleCalculator
    {
        public static void Apply(CotizacionMetradoDetalle d, SistemaCalculoMetrado? sistema)
        {
            // OuterLength (LongitudExt_Cot) según Anchorage (Anclaje_Cot)
            d.OuterLength = d.Anchorage switch
            {
                TipoAnclaje.Si => d.InnerLength + d.Support,
                TipoAnclaje.No => d.InnerLength,
                TipoAnclaje.Medio => d.InnerLength + (d.Support / 2m),
                _ => d.InnerLength
            };

            // TotalMaterial (TotalMtl_Cot)
            d.TotalMaterial = d.OuterLength * d.Quantity * d.Times;

            // VaultCount (NBovedilla_Cot). WastePercentage acá es en realidad
            // Ancho_Alt de TAlturaLosa copiado a la línea (ver nota en
            // CotizacionMetradoDetalle), no un porcentaje real — se preserva
            // el nombre legacy. Guard de división por cero: sin este dato la
            // línea no se puede calcular, se deja en 0 en vez de lanzar.
            d.VaultCount = d.WastePercentage != 0
                ? (d.InnerLength - d.Widening) / d.WastePercentage
                : 0m;

            // QuantityB (CantidadB_Cot)
            d.QuantityB = d.VaultCount * d.Row * d.Times * d.Support2;

            // Area (Area_Pre)
            d.Area = d.InnerLength * d.Width * d.Times;

            // QuantityP (CantidadP_Cot): difiere entre V1 y V2
            var quantityP = d.InnerLength * d.Times * d.Row;
            d.QuantityP = sistema == SistemaCalculoMetrado.V1 && d.PolystyrenePrice == 0
                ? 0m
                : quantityP;

            // Quantity (Cantidad_Cot): SOLO se recalcula en V1 (Flag_Tipo
            // distinto de '1'); en V2 el valor queda como lo ingresó el
            // usuario (ver ue_actualizavalores1, que no la toca).
            if (sistema != SistemaCalculoMetrado.V2 && d.Spacing > 0)
            {
                d.Quantity = Math.Ceiling(d.Width / d.Spacing);
            }

            // Row (Fila_Cot): la fórmula real llama a dbo.Filas(), función
            // SQL no portada todavía — se mantiene el valor de entrada tal
            // cual (manual) en ambos sistemas hasta poder confirmarla.
        }
    }
}
