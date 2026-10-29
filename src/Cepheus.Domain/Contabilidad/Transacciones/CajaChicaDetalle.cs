using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Línea de caja chica: un comprobante pagado con la caja. Importes en la moneda del comprobante.
    ///
    /// Legacy: dbo.MCajaChicaDetalle. PK legacy (Codigo_chi, Item_chi) -> (CajaCode, ItemNumber).
    ///   Item_chi          -> ItemNumber (correlativo, int)
    ///   Opcion_Ing        -> Origen (I Manual / N NotaIngreso / S OrdenServicio)
    ///   Codigo_noi        -> ReferenceCode (código de la Nota de Ingreso u Orden de Servicio; opcional)
    ///   Codigo_pla        -> PlantaCode (planta del comprobante / de la nota u orden referida)
    ///   Codigo_tdo, Numero_doc, Fecemision_doc -> ComprobantePagoCode, NumeroDocumento, FechaEmision
    ///   Moneda_doc        -> MonedaCode (char(1) legacy -> FK Comunes.Moneda, ISO 4217)
    ///   TCambio_doc       -> TipoCambio (tipo de cambio venta de la moneda a la fecha de emisión; 1 para la moneda base)
    ///   Cantidad_doc, PrecioUnidad_doc -> Cantidad, PrecioUnidad
    ///   Parcial_doc       -> Parcial (cantidad x precio; columna calculada en el legacy, aquí se persiste)
    ///   Igv_doc, NoGravable_doc, IRenta_doc, ISolidaridad_doc, Servicio_doc, IgvExterior_doc
    ///                     -> Igv, NoGravable, Renta, Fonavi (ISolidaridad), Servicio, IgvExterior
    ///   Total_doc         -> Total = Parcial + Igv - Renta - Fonavi + Servicio + IgvExterior
    ///   Codigo_cta, Codigo_ane -> CuentaContableCode, AnexoTipoCode/AnexoCode (el tipo sale de la cuenta)
    ///   Concepto_doc      -> Concepto
    ///   Codigo_prv        -> ProveedorCode
    ///   Numero_tdo, NumeroC_doc, Fecha_doc (usados por el PowerBuilder y el SP de documentos para la
    ///   nota de crédito 07 y ausentes del DDL) -> DocumentoReferenciaComprobantePagoCode,
    ///   DocumentoReferenciaNumero, DocumentoReferenciaFecha
    ///   ruc_prv           -> no se guarda: se obtiene del proveedor.
    /// </summary>
    public class CajaChicaDetalle
    {
        public string CajaCode { get; set; } = default!;
        public CajaChica Caja { get; set; } = default!;

        public int ItemNumber { get; set; }

        public OrigenLineaCajaChica Origen { get; set; } = OrigenLineaCajaChica.Manual;
        public string? ReferenceCode { get; set; }

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string ComprobantePagoCode { get; set; } = default!;
        public ComprobantePago ComprobantePago { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public DateTime FechaEmision { get; set; }

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;
        public decimal TipoCambio { get; set; }

        public decimal Cantidad { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal Parcial { get; set; }
        public decimal Igv { get; set; }
        public decimal NoGravable { get; set; }
        public decimal Renta { get; set; }
        public decimal Fonavi { get; set; }
        public decimal Servicio { get; set; }
        public decimal IgvExterior { get; set; }
        public decimal Total { get; set; }

        public string CuentaContableCode { get; set; } = default!;
        public CuentaContable CuentaContable { get; set; } = default!;
        public string? AnexoTipoCode { get; set; }
        public string? AnexoCode { get; set; }
        public Anexo? Anexo { get; set; }

        public string? Concepto { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string? DocumentoReferenciaComprobantePagoCode { get; set; }
        public string? DocumentoReferenciaNumero { get; set; }
        public DateTime? DocumentoReferenciaFecha { get; set; }
    }
}
