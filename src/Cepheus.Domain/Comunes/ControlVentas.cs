using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Parámetros de control de ventas/compras (IGV, retenciones, detracción,
    /// topes de días, fechas de proceso/cierre). Configuración global compartida
    /// entre módulos (Ventas, Compras, Contabilidad).
    ///
    /// Legacy: dbo.TControlesVentas (SQL Server, base "Administracion").
    ///
    /// DIFERENCIA DE DISEÑO respecto a TODOS los catálogos anteriores: esta tabla
    /// NO es un catálogo de muchos registros (Planta, Moneda, TipoDocumento...),
    /// es una fila ÚNICA de configuración global del sistema (patrón "singleton
    /// settings row"). El legacy la modela igual: PK autoincremental "control"
    /// pero en la práctica solo existe/se usa un registro vigente. Por eso:
    ///   - No hay Create (se siembra una única vez vía seed/migración).
    ///   - No hay listado paginado ni búsqueda.
    ///   - No hay IsActive/ToggleStatus (no aplica a una configuración única).
    ///   - Solo expone GetCurrent + Update.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Porcentaje_igv      -> IgvPercentage
    ///   Porcentaje_ret      -> WithholdingPercentage
    ///   Anomes_cierre /
    ///   AnnoMes_cie         -> ClosingPeriod        (el legacy tenía DOS columnas
    ///                           casi idénticas para el mismo dato: se consolidan
    ///                           en una sola, formato "YYYYMM")
    ///   FecPro_ventas       -> SalesProcessDate
    ///   FecPro_compras      -> PurchasesProcessDate
    ///   FecCan_ventas       -> SalesCancelDate
    ///   FecCan_compras      -> PurchasesCancelDate
    ///   tope_ret            -> WithholdingCap
    ///   porcentaje_det      -> DetractionPercentage
    ///   porcentaje_ren      -> IncomeTaxPercentage
    ///   porcentaje_fon      -> FonaviPercentage      (derogado en Perú, ver nota)
    ///   porcentaje_igvext   -> ForeignIgvPercentage
    ///   porcentaje_cua      -> QuotaPercentage        (significado ambiguo en el
    ///                           legacy — sin documentación de negocio disponible
    ///                           en el script; se mantiene el nombre literal
    ///                           "Cuota" hasta que confirmes su uso real)
    ///   Bloquea_Agr         -> BlocksGrouping
    ///   tope_retFac         -> WithholdingCapInvoice
    ///   TOPE_DIASOT         -> WorkOrderDaysLimit
    ///   TOPE_DIASMAXOT      -> WorkOrderMaxDays
    ///   TOPE_DIASMAXVTA     -> SaleMaxDays
    ///   TOPE_SALDO          -> BalanceLimit
    ///   num_dias_adic_activacion -> AdditionalActivationDays
    ///   IND_SERVICIO        -> IsServiceIndicator
    ///
    /// Legacy EXCLUIDO deliberadamente (metadata de aplicación/versión legacy,
    /// no son parámetros de negocio):
    ///   numero_val, numero_let, version_nro, version_n_eqp, feccontrol_ord,
    ///   NUM_VERSION_APP, IMP_DIF_RPT_OTT, Perfil_Correo, COD_TIP_LOGOS,
    ///   CAN_MIN_VACIADO.
    ///
    /// Nota: porcentaje_fon (Fonavi) se mantiene solo por continuidad histórica
    /// (mismo criterio que TipoDocumento.AffectsFonavi); Fonavi fue derogado
    /// (Ley 26969, 1998) y no debería usarse en lógica nueva.
    /// </summary>
    public class ControlVentas : IAuditableEntity
    {
        public int Id { get; set; }

        public decimal IgvPercentage { get; set; }
        public decimal WithholdingPercentage { get; set; }
        public string ClosingPeriod { get; set; } = default!;

        public DateTime SalesProcessDate { get; set; }
        public DateTime PurchasesProcessDate { get; set; }
        public DateTime SalesCancelDate { get; set; }
        public DateTime PurchasesCancelDate { get; set; }

        public decimal WithholdingCap { get; set; }
        public decimal DetractionPercentage { get; set; }
        public decimal IncomeTaxPercentage { get; set; }
        public decimal FonaviPercentage { get; set; }
        public decimal ForeignIgvPercentage { get; set; }
        public decimal QuotaPercentage { get; set; }

        public bool BlocksGrouping { get; set; }
        public decimal WithholdingCapInvoice { get; set; }

        public int WorkOrderDaysLimit { get; set; }
        public int WorkOrderMaxDays { get; set; }
        public int SaleMaxDays { get; set; }
        public int? BalanceLimit { get; set; }
        public int? AdditionalActivationDays { get; set; }

        public bool IsServiceIndicator { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}