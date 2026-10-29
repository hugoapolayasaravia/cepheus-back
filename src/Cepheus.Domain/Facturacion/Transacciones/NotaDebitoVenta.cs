using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Nota de Débito de Ventas (cabecera). Aumenta la deuda del cliente (por ejemplo intereses,
    /// gastos de un cheque rebotado o de una letra).
    ///
    /// Legacy: dbo.RDebVentas. PK legacy (Numero_dve) -> PK compuesta (PlantaCode, Number):
    /// la numeración pertenece a la planta (Planta.NumeroDve, formato "000-00000").
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pla          -> PlantaCode (FK -> Comunes.Planta)
    ///   Numero_dve          -> Number
    ///   FecEmision_dve      -> IssueDate
    ///   Codigo_cli, Codigo_obr -> ClienteCode, ObraCode (FK compuesta -> Obra)
    ///   Codigo_ven          -> VendedorCode (opcional)
    ///   DirEntrega_dve, UbiDirEntrega_dve -> DeliveryAddress, DeliveryUbigeoCode
    ///   Impresion_dve, FecImpresion_dve   -> IsPrinted, PrintedAt (S/N -> bool)
    ///   Codigo_pgv          -> FormaPagoVentaCode
    ///   Moneda_dve          -> MonedaCode (char(1) S/D -> ISO 4217 PEN/USD)
    ///   TipCambio           -> ExchangeRate (tipo de cambio venta a la fecha de emisión; 1 para PEN)
    ///   Codigo_ped          -> OrderCode
    ///   Glosa_dve           -> Description
    ///   codigo_pla_eje      -> ExecutingPlantaCode (planta ejecutora)
    ///   TotBrutoSol_dve     -> SubTotal
    ///   TotIgvSol_dve       -> IgvAmount
    ///   TotNetoSol_dve      -> TotalAmount
    ///   Pendientesol_dve    -> PendingAmount (saldo por cobrar; lo mueve el módulo de Cancelaciones)
    ///   (los importes en dólares del legacy se eliminan: el documento guarda una sola moneda + tipo de cambio)
    ///   AsiProvision_dve    -> ProvisionEntryNumber (asiento de provisión, tipo "03")
    ///   AsiAnulacion_dve    -> CancellationEntryNumber
    ///   FecAnulacion_dve    -> CanceledAt
    ///   FecCancelacion_dve  -> SettledAt (fecha en que quedó cobrada por completo)
    ///   Tipo_dve            -> Type (siempre "N" en el legacy)
    ///   Codigo_tdo, Numero_doc -> DocumentTypeCode, DocumentNumber: documento que se debita
    ///                          (factura 01, boleta 03, letra 10, cheque 11) o sin documento
    ///                          (null; el legacy usa "00")
    ///   Estado              -> Status (enum EstadoNotaDebito)
    ///   Clave, FechaFinal_val, FecPago_dve, TipCamCdu, CobranzaDudosa -> NO se migran (no los usa la ventana)
    ///   FecProceso_dve, codigo_usu -> ELIMINADOS: auditoría (CreatedAt/CreatedBy)
    ///   (nuevo) CancelReason: motivo de anulación.
    /// </summary>
    public class NotaDebitoVenta : IAuditableEntity
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
        public DateTime? SettledAt { get; set; }

        public string Type { get; set; } = "N";

        /// <summary>Documento que se debita (null = sin documento). Ver resumen de la clase.</summary>
        public string? DocumentTypeCode { get; set; }
        public ComprobantePago? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }

        public EstadoNotaDebito Status { get; set; } = EstadoNotaDebito.Pendiente;

        public ICollection<NotaDebitoVentaDetalle> Details { get; set; } = new List<NotaDebitoVentaDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
