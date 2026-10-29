using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Comprobante de pago de ventas: Factura (01) y Boleta (03) en una sola entidad. Clave: TypeCode + Number.
    /// Legacy: RFacVentas / RBolVentas (+ DFacVentas / DBolVentas).
    ///
    ///   (tabla de origen)          -> TypeCode ("01" Factura | "03" Boleta; los mismos códigos de RGuiVentas.codigo_tdo)
    ///   Numero_fve / Numero_bve    -> Number ("SSS-NNNNNN")
    ///   Codigo_pla                 -> PlantCode (planta que emite);  codigo_pla_eje -> ExecutingPlantCode
    ///   FecEmision_*               -> IssueDate
    ///   Codigo_cli, Codigo_obr, Codigo_ven -> ClienteCode, ObraCode, VendedorCode
    ///   DirEntrega_*, UbiDirEntrega_*      -> Address, UbigeoCode
    ///   Codigo_pgv                 -> PaymentFormCode
    ///   moneda_* (S/D)             -> CurrencyCode (PEN/USD);  TipCambio -> ExchangeRate (venta del día de emisión)
    ///   codigo_ped                 -> OrdenTrabajoCode (null en comprobantes libres)
    ///   tipo_fve                   -> Origin (Guias | Libre) + DocumentFlow (el codigo_tip de la aprobación) + IsFreeTransfer ('R')
    ///   glosa_*                    -> Glosa
    ///   TotBruto/TotIgv/TotNeto    -> SubtotalAmount, IgvAmount, TotalAmount (en la moneda del comprobante; el PB
    ///                                 guardaba solo soles) y TotalSoles (equivalente en soles)
    ///   Impresion_*, Fec impresión -> IsPrinted, PrintedAt
    ///   AsiProvision_*             -> ProvisionEntry (asiento de provisión, lo genera Contabilidad)
    ///   documento_bco, monto_bco   -> DetractionDocument, DetractionAmount
    ///   Documento_Caj              -> CashDocument
    ///   Estado                     -> Status (01 Pendiente | 02 Facturado | 03 Cancelado | 04 Anulado)
    ///   FecAnulacion_*             -> CanceledAt (+ CanceledBy, CancelReason, ReplacedByNumber: nuevos)
    ///
    /// NO se migran: PendienteSol/FecPago/FechaFinal_val/FecCancelacion (cobranzas), clave, CobranzaDudosa, TipCamCdu,
    /// PaseFox, AsiAnulacion (Contabilidad), y los datos de transporte (Codigo_Tra/Cho/Veh, P_Origen/P_Llegada: están en la guía).
    /// </summary>
    public class ComprobanteVenta : IAuditableEntity
    {
        public string TypeCode { get; set; } = default!;
        public string Number { get; set; } = default!;

        public string PlantCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;
        public string ExecutingPlantCode { get; set; } = default!;

        public DateTime IssueDate { get; set; }

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;
        public string ObraCode { get; set; } = default!;
        public Obra Obra { get; set; } = default!;
        public string VendedorCode { get; set; } = default!;
        public Vendedor Vendedor { get; set; } = default!;

        public string Address { get; set; } = default!;
        public string UbigeoCode { get; set; } = default!;
        public Ubigeo Ubigeo { get; set; } = default!;

        public string PaymentFormCode { get; set; } = default!;
        public FormaPagoVenta PaymentForm { get; set; } = default!;

        public string CurrencyCode { get; set; } = "PEN";
        public decimal ExchangeRate { get; set; } = 1m;

        public string? OrdenTrabajoCode { get; set; }
        public OrdenTrabajoVenta? OrdenTrabajo { get; set; }

        public OrigenComprobanteVenta Origin { get; set; } = OrigenComprobanteVenta.Guias;
        public FlujoDocumentoAprobacion? DocumentFlow { get; set; }
        public bool IsFreeTransfer { get; set; }

        public string? Glosa { get; set; }

        public decimal IgvPercentage { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalSoles { get; set; }

        public EstadoComprobanteVenta Status { get; set; } = EstadoComprobanteVenta.Pendiente;

        public bool IsPrinted { get; set; }
        public DateTime? PrintedAt { get; set; }
        public string? ProvisionEntry { get; set; }

        public string? DetractionDocument { get; set; }
        public decimal DetractionAmount { get; set; }
        public string? CashDocument { get; set; }

        public DateTime? CanceledAt { get; set; }
        public string? CanceledBy { get; set; }
        public string? CancelReason { get; set; }
        public string? ReplacedByNumber { get; set; }

        public ICollection<ComprobanteVentaDetalle> Detalles { get; set; } = new List<ComprobanteVentaDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
