using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Línea de un asiento contable. Legacy: dbo.MAsientoDetalle.
    /// PK: (Anio, Mes, TipoAsientoCode, Numero, ItemNumber).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Item_res        -> ItemNumber
    ///   Codigo_pla      -> PlantaCode (FK Comunes.Planta)
    ///   Destino_det     -> EsDestino ('S' = línea generada como destino; 'N' = línea normal)
    ///   Descripcion_det -> Descripcion (50)
    ///   Codigo_cta      -> CuentaContableCode (FK Contabilidad.CuentaContable)
    ///   Codigo_tip + Codigo_ane -> AnexoTipoCode + AnexoCode (FK compuesta a Anexo; opcional)
    ///   Numero_doc      -> NumeroDocumento (15)
    ///   CargosSol_det / AbonosSol_det -> CargoSol / AbonoSol
    ///   Tipo_con        -> TipoContabilidad
    ///   planta_eje      -> PlantaEjecutoraCode (FK Comunes.Planta; opcional)
    ///   Codigo_tdo      -> ComprobantePagoCode (FK Comunes.ComprobantePago; opcional)
    ///
    /// Los importes en dólares no se guardan (CargoDol/AbonoDol se calculan al consultar con el TipoCambio
    /// del asiento). Codigo_ciu desaparece (no existe en el DDL nuevo).
    /// </summary>
    public class AsientoContableDetalle : IAuditableEntity
    {
        public string Anio { get; set; } = default!;
        public string Mes { get; set; } = default!;
        public string TipoAsientoCode { get; set; } = default!;
        public string Numero { get; set; } = default!;
        public AsientoContable Asiento { get; set; } = default!;

        public int ItemNumber { get; set; }

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public bool EsDestino { get; set; }

        public string Descripcion { get; set; } = default!;

        public string CuentaContableCode { get; set; } = default!;
        public CuentaContable CuentaContable { get; set; } = default!;

        public string? AnexoTipoCode { get; set; }
        public string? AnexoCode { get; set; }
        public Anexo? Anexo { get; set; }

        public string? NumeroDocumento { get; set; }

        public decimal CargoSol { get; set; }
        public decimal AbonoSol { get; set; }

        public TipoContabilidad TipoContabilidad { get; set; } = TipoContabilidad.Todos;

        public string? PlantaEjecutoraCode { get; set; }
        public Planta? PlantaEjecutora { get; set; }

        public string? ComprobantePagoCode { get; set; }
        public ComprobantePago? ComprobantePago { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
