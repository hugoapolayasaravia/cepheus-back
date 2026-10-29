using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Logistica.Catalogos;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Nota de Crédito de Ventas (cabecera). Puede originarse en una Solicitud de Nota
    /// de Crédito aprobada o registrarse directamente.
    ///
    /// Legacy: dbo.RCreVentas. PK legacy (Numero_cve) -> PK compuesta (PlantaCode, Number):
    /// la numeración pertenece a la planta (Planta.NumeroCve, formato "000-00000").
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pla          -> PlantaCode (FK -> Comunes.Planta)
    ///   Numero_cve          -> Number
    ///   FecEmision_cve      -> IssueDate
    ///   Codigo_cli, Codigo_obr -> ClienteCode, ObraCode (FK compuesta -> Obra)
    ///   Codigo_ven          -> VendedorCode (opcional)
    ///   DirEntrega_cve, UbiDirEntrega_cve -> DeliveryAddress, DeliveryUbigeoCode
    ///   FecImpresion_cve, impresion_cve   -> PrintedAt, IsPrinted (S/N -> bool)
    ///   Codigo_pgv          -> FormaPagoVentaCode
    ///   Moneda_cve          -> MonedaCode (char(1) -> FK Comunes.Moneda, ISO 4217)
    ///   TipCambio (DF)      -> ExchangeRate (tipo de cambio venta de la moneda a la
    ///                          fecha de emisión; 1 para la moneda base)
    ///   Codigo_ped          -> OrderCode (Orden de Pedido, código simple)
    ///   Glosa_cve           -> Description
    ///   codigo_pla_eje      -> ExecutingPlantaCode (planta ejecutora)
    ///   TotBrutoSol_cve     -> SubTotal
    ///   TotIgvSol_cve       -> IgvAmount
    ///   TotNetoSol_cve      -> TotalAmount
    ///     (los comentarios del DDL legacy rotulan estas tres columnas de forma
    ///      desplazada; se siguió el uso real del PowerBuilder: bruto=sub total,
    ///      igv=igv, neto=total)
    ///   Pendientesol_cve (DF) -> PendingAmount (saldo por cobrar/aplicar)
    ///   AsiProvision_cve    -> ProvisionEntryNumber (asiento de provisión; lo genera
    ///                          Contabilidad, aún no integrado)
    ///   AsiAnulacion_cve, FecAnulacion_cve -> CancellationEntryNumber, CanceledAt
    ///   Tipo_cve            -> Type (char(1): N, O, P, C; significado no documentado)
    ///   Codigo_tdo, Numero_doc -> DocumentTypeCode (FK ComprobantePago), DocumentNumber
    ///                          (comprobante que se acredita)
    ///   Codigo_dto          -> MotivoDevolucionCode (FK -> Comunes.MotivoDevolucion)
    ///   Ano_snc, Mes_snc, Codigo_snc -> SolicitudYear/Month/Code (FK compuesta opcional)
    ///   Documento_Caj       -> CashDocument (se mantiene)
    ///   Estado              -> Status (enum EstadoNotaCredito)
    ///   FecProceso_cve, codigo_usu -> ELIMINADOS: auditoría (CreatedAt/CreatedBy)
    ///   (nuevo) CancelReason: motivo de anulación.
    /// </summary>
    public class NotaCreditoVenta : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string Number { get; set; } = default!;

        public DateTime IssueDate { get; set; }

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        public string ObraCode { get; set; } = default!;
        public Obra Obra { get; set; } = default!;

        public string? VendedorCode { get; set; }
        public Vendedor? Vendedor { get; set; }

        public string DeliveryAddress { get; set; } = default!;
        public string DeliveryUbigeoCode { get; set; } = default!;
        public Ubigeo DeliveryUbigeo { get; set; } = default!;

        public DateTime? PrintedAt { get; set; }
        public bool IsPrinted { get; set; }

        public string FormaPagoVentaCode { get; set; } = default!;
        public FormaPagoVenta FormaPagoVenta { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;
        public decimal ExchangeRate { get; set; }

        public string? OrderCode { get; set; }

        public string Description { get; set; } = default!;

        public string ExecutingPlantaCode { get; set; } = default!;
        public Planta ExecutingPlanta { get; set; } = default!;

        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public string? CancellationEntryNumber { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancelReason { get; set; }

        public string Type { get; set; } = "N";

        public string DocumentTypeCode { get; set; } = default!;
        public ComprobantePago DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;

        public string MotivoDevolucionCode { get; set; } = default!;
        public MotivoDevolucionArticulo MotivoDevolucion { get; set; } = default!;

        public string? SolicitudCode { get; set; }
        public string? SolicitudYear { get; set; }
        public string? SolicitudMonth { get; set; }
        public SolicitudNotaCredito? Solicitud { get; set; }

        public string? CashDocument { get; set; }

        public EstadoNotaCredito Status { get; set; } = EstadoNotaCredito.Pendiente;

        public ICollection<NotaCreditoVentaDetalle> Details { get; set; } = new List<NotaCreditoVentaDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
