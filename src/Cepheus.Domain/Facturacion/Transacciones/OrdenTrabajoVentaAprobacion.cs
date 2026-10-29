using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Aprobación de Créditos de la OT. Legacy: programacion.dbo.Aprobaciones.
    ///
    /// El legacy guarda una fila POR LÍNEA de la OT (PK Clave = PedidoVentaDetalle.Clave) repitiendo los mismos
    /// datos del encabezado, con la cantidad aprobada de cada línea (Cantidad_cre). Aquí es 1:1 con la OT (PK =
    /// OrdenTrabajoCode) y la cantidad aprobada de cada línea vive en OrdenTrabajoVentaDetalle.ApprovedQuantity.
    /// La opción del PB "aprobar solo esta línea" (chequeat = 0) no se migra.
    ///
    ///   Documento, numero_doc2   -> Document1Number, Document2Number ("SSS-NNNNNN", char(10))
    ///   Tipo_doc                 -> DocumentType (F/B)
    ///   Cantidad_cre             -> ApprovedQuantity (suma de las líneas)
    ///   FecHor_cre               -> ApprovedAt
    ///   Tipo_pag                 -> PaymentType (R/C)
    ///   Estado_cre               -> CreditStatus (A/R)
    ///   codigo_tip               -> DocumentFlow (N/P/G/C)
    ///   dias_val                 -> ValorizationCode (tipo de valorización o "N")
    ///   Codigo (char 1)          -> ValorizationWeekday (día de la semana si la valorización es semanal)
    ///   numero (int)             -> ValorizationDays (días si la valorización es por días)
    ///   documentos_flg           -> HasTwoDocuments
    ///   Codigo_pla1/Codigo_pla2  -> Plant1Code/Plant2Code
    ///   Tipo_gve (char 2)        -> GuideMotivoCode (motivo de las guías)
    ///   Codigo_tpa               -> PaymentMethodCode (tipo de pago: no hay catálogo en el modelo)
    ///   Codigo_Ban, Documento_Pag-> BankCode, PaymentDocumentNumber
    ///   Codigo_pgv               -> FormaPagoVentaCode
    ///   Creditos_obs             -> CreditsObservations
    /// </summary>
    public class OrdenTrabajoVentaAprobacion : IAuditableEntity
    {
        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajoVenta OrdenTrabajo { get; set; } = default!;

        public string? CreditsObservations { get; set; }

        public string? Document1Number { get; set; }
        public TipoDocumentoAprobacion DocumentType { get; set; }

        public decimal ApprovedQuantity { get; set; }
        public DateTime ApprovedAt { get; set; }

        public TipoPagoAprobacion PaymentType { get; set; }
        public EstadoCreditoAprobacion CreditStatus { get; set; } = EstadoCreditoAprobacion.Aprobado;
        public FlujoDocumentoAprobacion DocumentFlow { get; set; } = FlujoDocumentoAprobacion.SinDocumento;

        public string ValorizationCode { get; set; } = "N";
        public DiaSemana? ValorizationWeekday { get; set; }
        public int? ValorizationDays { get; set; }

        public string? Document2Number { get; set; }
        public bool HasTwoDocuments { get; set; }

        public string? Plant1Code { get; set; }
        public Planta? Plant1 { get; set; }

        public string? Plant2Code { get; set; }
        public Planta? Plant2 { get; set; }

        public string? GuideMotivoCode { get; set; }

        public string? PaymentMethodCode { get; set; }

        public string? BankCode { get; set; }
        public Banco? Bank { get; set; }

        public string? PaymentDocumentNumber { get; set; }

        public string FormaPagoVentaCode { get; set; } = default!;
        public FormaPagoVenta FormaPagoVenta { get; set; } = default!;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
