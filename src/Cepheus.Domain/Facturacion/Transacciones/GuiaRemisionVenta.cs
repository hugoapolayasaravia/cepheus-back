using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Guía de remisión de ventas. Legacy: dbo.RGuiVentas (+ DGuiVentas).
    ///
    ///   Numero_gve               -> Number (PK, "SSS-NNNNNN"; correlativo de Planta.NumeroGve)
    ///   Codigo_pla               -> PlantCode (la planta de la programación; sin OT, la elegida)
    ///   FecEmision               -> IssueDate (siempre la fecha del sistema)
    ///   Hora_gve                 -> DispatchTime ("HH:mm")
    ///   Codigo_mot               -> Motive (01 Venta | 02 Traslado | 10 Devolución | 13 Reposición)
    ///   Codigo_cli, Codigo_obr   -> ClienteCode, ObraCode;  Codigo_ven -> VendedorCode
    ///   Direccion_gve, direccion_ubi_gve -> Address, UbigeoCode
    ///   Descripcion_gve          -> Description
    ///   (Codigo_tra, Codigo_cho) -> (TransportistaCode, ChoferCode);  (Codigo_tra, tipo_veh, codigo_veh) -> vehículo
    ///   Estado                   -> Status (01 Pendiente | 02 Facturado | 03 Cancelado | 04 Anulado | 10 | 13)
    ///   tipo_gve                 -> DocumentFlow (el codigo_tip de la aprobación de la OT: N/P/G/C)
    ///   codigo_tdo               -> DocumentTypeCode ("00" en espera, "01" Factura, "03" Boleta; null en traslado/devolución)
    ///   numero_doc1, numero_doc2 -> Document1Number, Document2Number (se completan al emitir el comprobante)
    ///   codigo_ped               -> OrdenTrabajoCode (null = guía "en espera", sin OT)
    ///   codigo_pro               -> ProgramacionType + ProgramacionCode
    ///   codigo_fle, monto_fle    -> FleteCode, FleteAmount
    ///   numero_dev               -> ReplacesNumber (guía que se repone; motivo 13)
    ///   cod_motivo_dev           -> DevolutionMotiveCode;  COD_ACCION_DEV -> DevolutionActionCode ("01" = vuelve al saldo de la OT)
    ///   tipcambio_gve, moneda_gve-> ExchangeRate (venta del día), CurrencyCode
    ///   numero_tic, Bomba_gve    -> TicketNumber, PumpCode
    ///   Impresion_gve            -> IsPrinted;  saldo_gve ('S') -> IsBalance
    ///   FecAnulacion, Usuario_anu-> CanceledAt, CanceledBy (+ CancelReason, nuevo)
    ///
    /// NO se migran (no los usa ninguna regla entregada o los reemplaza otra tabla): Transportista char(11) ("No debe ir"),
    /// bonificación, motivo_gui, Suple_sn, documento_bco/monto_bco (cobro al contado: va con los comprobantes),
    /// numero_val, Fecha_apr/Login_apr (la aprobación de anulación es el permiso GUIASREMISIONVENTA.ANNUL),
    /// FecProceso/Codigo_usu (auditoría).
    /// </summary>
    public class GuiaRemisionVenta : IAuditableEntity
    {
        public string Number { get; set; } = default!;

        public string PlantCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public DateTime IssueDate { get; set; }
        public string? DispatchTime { get; set; }

        public MotivoGuiaRemisionVenta Motive { get; set; } = MotivoGuiaRemisionVenta.Venta;

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;
        public string ObraCode { get; set; } = default!;
        public Obra Obra { get; set; } = default!;
        public string VendedorCode { get; set; } = default!;
        public Vendedor Vendedor { get; set; } = default!;

        public string Address { get; set; } = default!;
        public string UbigeoCode { get; set; } = default!;
        public Ubigeo Ubigeo { get; set; } = default!;
        public string Description { get; set; } = default!;

        public string TransportistaCode { get; set; } = default!;
        public TransportistaVenta Transportista { get; set; } = default!;
        public string ChoferCode { get; set; } = default!;
        public ChoferVenta Chofer { get; set; } = default!;
        public TipoVehiculo? VehicleType { get; set; }
        public string? VehicleCode { get; set; }
        public VehiculoVenta? Vehiculo { get; set; }

        public EstadoGuiaRemisionVenta Status { get; set; } = EstadoGuiaRemisionVenta.Pendiente;

        public FlujoDocumentoAprobacion? DocumentFlow { get; set; }
        public string? DocumentTypeCode { get; set; }
        public string? Document1Number { get; set; }
        public string? Document2Number { get; set; }

        public string? OrdenTrabajoCode { get; set; }
        public OrdenTrabajoVenta? OrdenTrabajo { get; set; }
        public TipoProgramacionGuia? ProgramacionType { get; set; }
        public string? ProgramacionCode { get; set; }

        public string FleteCode { get; set; } = default!;
        public Flete Flete { get; set; } = default!;
        public decimal FleteAmount { get; set; }

        public string? ReplacesNumber { get; set; }
        public string? DevolutionMotiveCode { get; set; }
        public MotivoDevolucion? DevolutionMotive { get; set; }
        public string? DevolutionActionCode { get; set; }

        public string CurrencyCode { get; set; } = "PEN";
        public decimal ExchangeRate { get; set; } = 1m;

        public string? TicketNumber { get; set; }
        public string? PumpCode { get; set; }

        public bool IsPrinted { get; set; }
        public DateTime? PrintedAt { get; set; }
        public bool IsBalance { get; set; }

        public DateTime? CanceledAt { get; set; }
        public string? CanceledBy { get; set; }
        public string? CancelReason { get; set; }

        public ICollection<GuiaRemisionVentaDetalle> Detalles { get; set; } = new List<GuiaRemisionVentaDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
