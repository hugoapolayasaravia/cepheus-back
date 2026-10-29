using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Tipo de cambio diario (compra/venta), usado para el control de compras y
    /// ventas en moneda extranjera. Catálogo maestro compartido entre módulos
    /// (Ventas, Compras, Contabilidad).
    ///
    /// Legacy: dbo.Ttipcambio (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Fecha_tca   -> Date         (era la PK en el legacy; acá se mantiene como
    ///                                 clave de negocio única, con Id surrogate como
    ///                                 PK técnica, igual que el resto de entidades
    ///                                 de Comunes)
    ///   venta_tca   -> SellRate     (tipo de cambio venta)
    ///   compra_tca  -> BuyRate      (tipo de cambio compra)
    ///
    /// DIFERENCIA DE DISEÑO respecto a los catálogos anteriores (Planta, Moneda,
    /// TipoDocumento, ComprobantePago, Ubigeo): esta NO es una entidad catálogo
    /// (código + nombre + activo/inactivo), sino un REGISTRO HISTÓRICO por fecha.
    /// Por eso:
    ///   - No tiene IsActive ni Toggle-status: un tipo de cambio de una fecha
    ///     pasada no se "desactiva", es un hecho histórico.
    ///   - No tiene Code: la clave de negocio es la fecha (Date), única por día.
    ///   - Se agrega GetByDate además de GetById, porque el caso de uso típico
    ///     es "dame el tipo de cambio del día X" (ej. al emitir un comprobante
    ///     en moneda extranjera), no navegar por Id.
    ///
    /// Legacy EXCLUIDO: el legacy no distingue por moneda (se asume implícito
    /// USD/PEN, el par más común en Perú). Si en el futuro se requiere manejar
    /// tipo de cambio para más de un par de monedas, se debería agregar una
    /// referencia a Comunes.Moneda (ej. MonedaOrigenId/MonedaDestinoId). Por
    /// ahora se mantiene igual al legacy (un solo par implícito) para no
    /// introducir un cambio de alcance no solicitado.
    /// </summary>
    public class TipoCambio : IAuditableEntity
    {
        public int Id { get; set; }

        /// <summary>Moneda extranjera a la que corresponde el tipo de cambio (ISO 4217: USD, EUR...). Expresa cuántos soles vale una unidad de esa moneda.</summary>
        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public DateOnly Date { get; set; }
        public decimal SellRate { get; set; }
        public decimal BuyRate { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}