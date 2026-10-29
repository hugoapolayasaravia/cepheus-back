using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Ítem de depósito de un lote de detracción: un proveedor, un tipo de operación y un tipo de bien.
    /// Cada ítem es una línea del archivo de depósito masivo.
    ///
    /// Legacy: dbo.COM_DETRACCION_DET (SQL Server).
    /// PK legacy y actual: (Year, Period, LotNumber, ProveedorCode, ItemNumber).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_prv        -> ProveedorCode (FK Logistica.Proveedor)
    ///   NUM_ITEM          -> ItemNumber (correlativo dentro del lote)
    ///   COD_TIP_OPERACION -> TipoOperacionCode (FK Facturacion.TipoOperacion)
    ///   COD_TIPO_BIENES   -> TipoBienCode (FK Facturacion.TipoBien)
    ///   Ruc_prv           -> ProveedorRuc (copia del RUC al generar el lote)
    ///   COD_CTACTE_PROV   -> AccountNumber (copia de la cuenta de detracciones del proveedor al generar
    ///                        el lote) + ProveedorCuentaId (FK a la cuenta de origen)
    ///   IMP_DEPOSITO      -> DepositSol (suma de IMP_DETRACCION de los documentos marcados para exportar)
    /// </summary>
    public class DetraccionItem
    {
        public string Year { get; set; } = default!;
        public string Period { get; set; } = default!;
        public string LotNumber { get; set; } = default!;
        public Detraccion Detraccion { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public int ItemNumber { get; set; }

        public string TipoOperacionCode { get; set; } = default!;
        public TipoOperacion TipoOperacion { get; set; } = default!;

        public string TipoBienCode { get; set; } = default!;
        public TipoBien TipoBien { get; set; } = default!;

        public string ProveedorRuc { get; set; } = default!;

        /// <summary>Cuenta de detracciones del proveedor (Banco de la Nación) vigente al generar el lote.</summary>
        public string AccountNumber { get; set; } = default!;

        public int? ProveedorCuentaId { get; set; }
        public ProveedorCuenta? ProveedorCuenta { get; set; }

        public decimal DepositSol { get; set; }

        public ICollection<DetraccionDocumento> Documents { get; set; } = new List<DetraccionDocumento>();
    }
}
