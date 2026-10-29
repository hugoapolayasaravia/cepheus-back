using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Domain.Contabilidad.Catalogos
{
    /// <summary>
    /// Tipo de asiento contable. Catálogo del módulo de Contabilidad: define con qué
    /// tipo de cambio (compra o venta, de Comunes.TipoCambio) se valoriza el asiento y
    /// si es histórico o ajustado.
    ///
    /// Legacy: dbo.TTipAsiento (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tas        -> Code (PK natural, char(2), correlativo automático)
    ///   Descripcion_tas   -> Name (nvarchar(50) -> 100)
    ///   TipodeCambio_tas  -> ExchangeRateType (enum TipoCambioAplicable: 'C' Compra,
    ///                        'V' Venta; indica qué columna de Comunes.TipoCambio
    ///                        usa el asiento: BuyRate o SellRate)
    ///   Tipo_tas          -> Valuation (enum ValorizacionAsiento: 'H' Histórico,
    ///                        'A' Ajustado)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia.
    /// </summary>
    public class TipoAsiento : IAuditableEntity
    {
        /// <summary>Código del tipo de asiento (PK natural, 2 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>Tipo de cambio que usa el asiento (compra o venta).</summary>
        public TipoCambioAplicable ExchangeRateType { get; set; }

        /// <summary>Valorización del asiento (histórico o ajustado).</summary>
        public ValorizacionAsiento Valuation { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
