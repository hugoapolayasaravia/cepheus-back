using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Orden de Trabajo de Ventas (cabecera). Aggregate root: las líneas y la aprobación solo existen a
    /// través de la OT. Se llama OrdenTrabajoVenta para no chocar con la OT de Mantenimiento.
    ///
    /// Legacy: programacion.dbo.PedidoVentaResumen (PedidoResumen en el PB). Pantallas PB:
    /// w_ges_reg_ordenes_trabajo (cabecera), w_ges_reg_det_otrabajo (líneas), w_ges_otrabajo_aprobacion,
    /// w_ges_bus_cab_otrabajo_especial. Correlativo: fn_NuevaOrdenTrabajo (máximo + 1, global, char(8)).
    ///
    /// Mapeo de columnas legacy -> propiedades:
    ///   Codigo_ped            -> Code (PK)
    ///   Codigo_cli, Codigo_obr-> ClienteCode, ObraCode (FK compuesta -> Obra)
    ///   Fecha_des, Hora_des   -> DispatchDate, DispatchTime ("HH:mm")
    ///   Total_ped             -> Total (suma de las líneas no anuladas)
    ///   Tipo_prd              -> NO se guarda: se calcula de los negocios de las líneas (ProductTypes)
    ///   Observacion_ped       -> Observations
    ///   Codigo_ven            -> VendedorCode
    ///   moneda_cli            -> CurrencyCode (char(1) S/D, como la Cotización)
    ///   Codigo_mot            -> MotivoCode (FK -> Comunes.MotivoDevolucion)
    ///   Observaciones_cli     -> CreditObservations (solo el área de Créditos)
    ///   Pedido_Origen         -> OriginCode (auto-FK; OT de Anticipo en el tipo Descuento y OT origen al copiar)
    ///   Codigo_Neg, Ano_cot, Mes_cot, Codigo_cot -> CotizacionNegocioCode/Year/Month/Code (FK compuesta opcional)
    ///   Ind_Modificable       -> IsModifiable (S/N -> bool; false = bloqueada / enviada a aprobación)
    ///   Usuario_apr, Fecha_Apr, Observacion_Apr -> ApprovedBy, ApprovedAt, ApprovalObservations
    ///   Ind_PaseProduccion, FechaPaseProduccion -> PassedToProduction, ProductionPassDate
    ///   Usuario_Anula, Fecha_Anu, Observacion_Anu -> CanceledBy, CanceledAt, CancelReason
    ///   Usuario_Inactiva, Fecha_Ina, Observacion_Ina -> InactivatedBy, InactivatedAt, InactivationReason
    ///   Tipo_ped              -> Type
    ///   Observaciones_hc      -> CutSheetObservations
    ///   Codigo_est            -> Status (enum EstadoOrdenTrabajoVenta; 05/06/11/42)
    ///   Fecha_ped, Usuario    -> CreatedAt, CreatedBy (variables de auditoría)
    ///   Etapa_Ped             -> Stage (enum EtapaOrdenTrabajoVenta; P/A/C/G)
    ///
    /// Columnas que usa el PB y NO están en el DDL entregado, agregadas por ser necesarias para reglas del PB
    /// (pedir confirmación):
    ///   codigo_com            -> PurchaseOrderNumber (N° de Orden de Compra del cliente; obligatorio si el cliente
    ///                            lo exige — Cliente.RequiresPurchaseOrderApproval — en OT Normal/Anticipo; se limpia al anular)
    ///   fechor_ped            -> SentToApprovalAt (fecha-hora del bloqueo; base de la regla de 24 h de la aprobación)
    ///   Aprobacion_Ext, Usuario_Aprobo, Fecha_Aprobo, Observacion_Esp
    ///                         -> SpecialApproval* (excepción de línea de crédito concedida por Créditos)
    ///   motivopaseproduccion  -> ProductionPassReason
    ///   MotivoRetrasoAprobo   -> ApprovalDelayReason (motivo de demora si se aprueba un día después o más)
    /// </summary>
    public class OrdenTrabajoVenta : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public TipoOrdenTrabajoVenta Type { get; set; } = TipoOrdenTrabajoVenta.Normal;
        public EtapaOrdenTrabajoVenta Stage { get; set; } = EtapaOrdenTrabajoVenta.Pendiente;
        public EstadoOrdenTrabajoVenta Status { get; set; } = EstadoOrdenTrabajoVenta.Activo;

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        public string ObraCode { get; set; } = default!;
        public Obra Obra { get; set; } = default!;

        public DateTime DispatchDate { get; set; }
        public string DispatchTime { get; set; } = default!;

        public string VendedorCode { get; set; } = default!;
        public Vendedor Vendedor { get; set; } = default!;

        public string CurrencyCode { get; set; } = default!;

        public string MotivoCode { get; set; } = default!;
        public MotivoDevolucion Motivo { get; set; } = default!;

        public string? Observations { get; set; }
        public string CreditObservations { get; set; } = string.Empty;
        public string? CutSheetObservations { get; set; }

        public string? PurchaseOrderNumber { get; set; }

        public decimal Total { get; set; }

        public string? OriginCode { get; set; }
        public OrdenTrabajoVenta? Origin { get; set; }

        public string? CotizacionNegocioCode { get; set; }
        public string? CotizacionYear { get; set; }
        public string? CotizacionMonth { get; set; }
        public string? CotizacionCode { get; set; }
        public Cotizacion? Cotizacion { get; set; }

        public bool IsModifiable { get; set; } = true;
        public DateTime? SentToApprovalAt { get; set; }

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovalObservations { get; set; }
        public string? ApprovalDelayReason { get; set; }

        public bool PassedToProduction { get; set; }
        public DateTime? ProductionPassDate { get; set; }
        public string? ProductionPassReason { get; set; }

        public bool SpecialApprovalGranted { get; set; }
        public string? SpecialApprovalBy { get; set; }
        public DateTime? SpecialApprovalAt { get; set; }
        public string? SpecialApprovalObservations { get; set; }

        public string? CanceledBy { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancelReason { get; set; }

        public string? InactivatedBy { get; set; }
        public DateTime? InactivatedAt { get; set; }
        public string? InactivationReason { get; set; }

        // Hijos del aggregate
        public ICollection<OrdenTrabajoVentaDetalle> Detalles { get; set; } = new List<OrdenTrabajoVentaDetalle>();
        public OrdenTrabajoVentaAprobacion? Aprobacion { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
