using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Documento de Compra (registro de comprobantes de proveedores). Cabecera.
    ///
    /// Legacy: dbo.RDocCompras (SQL Server).
    /// PK legacy y actual: (Codigo_tdo, Numero_dco, Codigo_prv) =
    ///   (ComprobantePagoCode, NumeroDocumento, ProveedorCode).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tdo          -> ComprobantePagoCode (FK Comunes.ComprobantePago)
    ///   Numero_dco          -> NumeroDocumento (char(15))
    ///   Codigo_prv          -> ProveedorCode (FK Logistica.Proveedor)
    ///   FecEmision_dco      -> FechaEmision
    ///   FecRecepcion_dco    -> FechaRecepcion
    ///   fecha_doc           -> FechaDocumento (fecha del comprobante del proveedor; opcional)
    ///   Codigo_pgc          -> FormaPagoCode (FK Logistica.FormaPago)
    ///   Moneda_dco          -> MonedaCode (FK Comunes.Moneda, 3 chars)
    ///   Venta_dco           -> TipoCambioFecha: se guarda solo la FECHA del tipo de cambio;
    ///                          el valor (venta) se trae de Comunes.TipoCambio al consultar.
    ///   ParcialS_dco        -> ParcialSol (calculado: suma del detalle)
    ///   igvSol_dco          -> IgvSol (calculado con PeriodoIgv en la fecha de emisión)
    ///   NoGravableSol_dco   -> NoGravableSol
    ///   RentaSol_dco        -> RentaSol (calculado)
    ///   FonaviSol_dco       -> FonaviSol (calculado)
    ///   ServicioSol_dco     -> ServicioSol
    ///   IgvExtSol_dco       -> IgvExteriorSol
    ///   TotalS_dco          -> TotalSol
    ///   Glosa_dco           -> Glosa
    ///   Origen_flg          -> Origen (enum OrigenDocumentoCompra, default Contabilidad)
    ///   COD_TIPO_BIENES     -> TipoBienCode (FK Facturacion.TipoBien)
    ///   COD_TIP_OPERACION   -> TipoOperacionCode (FK Facturacion.TipoOperacion)
    ///   Operacion_Det / Constancia_Det / FecConstancia_Det
    ///                       -> OperacionDetraccion / ConstanciaDetraccion / FechaConstanciaDetraccion
    ///   numero_det / fecemision_det / montosol_det (legacy marcados "revisar si aún vigente",
    ///                          se conservan por indicación) -> NumeroDetraccion /
    ///                          FechaEmisionDetraccion / MontoDetraccionSol
    ///   codigo_doc + numero_doc -> DocumentoOrigenComprobantePagoCode + DocumentoOrigenNumero:
    ///                          documento origen de una nota de crédito (mismo proveedor);
    ///                          FK compuesta al propio DocumentoCompra.
    ///   Codigo_Com          -> OrdenCompraAdministracionCode (FK Logistica.OrdenCompraAdministracion)
    ///   fecha_pgc           -> FechaVencimiento (fecha de pago o vencimiento)
    ///   PagadoS_dco         -> PagadoSol
    ///   FecCancelacion_dco  -> FechaCancelacion
    ///   PendienteS_dco      -> PendienteSol (columna calculada: TotalSol - PagadoSol)
    ///   AsiProvision_dco    -> AsientoProvisionCode (sin FK: el asiento contable se genera después)
    ///   Procedencia_com     -> Procedencia (enum ProcedenciaDocumentoCompra, opcional)
    ///   Estado              -> Estado (enum EstadoDocumentoCompra)
    ///   Codigo_usu / FecProceso_dco -> cubiertos por CreatedBy / CreatedAt (auditoría)
    ///
    /// Cambios frente al legacy:
    ///   - Las FK a TipoBien y a Proveedor ya no hacen ON DELETE CASCADE (borrar un catálogo
    ///     no debe borrar documentos): Restrict.
    ///   - Los campos legacy NOT NULL que en la práctica van vacíos (Codigo_Com, AsiProvision,
    ///     numero_det) pasan a opcionales.
    /// </summary>
    public class DocumentoCompra : IAuditableEntity
    {
        public string ComprobantePagoCode { get; set; } = default!;
        public ComprobantePago ComprobantePago { get; set; } = default!;

        public string NumeroDocumento { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public DateTime FechaEmision { get; set; }
        public DateTime FechaRecepcion { get; set; }

        /// <summary>Fecha del comprobante del proveedor (legacy: RDocCompras.fecha_doc). Opcional.</summary>
        public DateTime? FechaDocumento { get; set; }

        public string FormaPagoCode { get; set; } = default!;
        public FormaPago FormaPago { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        /// <summary>Fecha del tipo de cambio aplicado (el valor se consulta en Comunes.TipoCambio). Null si es soles.</summary>
        public DateOnly? TipoCambioFecha { get; set; }

        public decimal ParcialSol { get; set; }
        public decimal IgvSol { get; set; }
        /// <summary>Digitado por el usuario.</summary>
        public decimal NoGravableSol { get; set; }
        public decimal RentaSol { get; set; }
        public decimal FonaviSol { get; set; }
        /// <summary>Digitado por el usuario.</summary>
        public decimal ServicioSol { get; set; }
        /// <summary>Digitado por el usuario.</summary>
        public decimal IgvExteriorSol { get; set; }
        public decimal TotalSol { get; set; }

        public string Glosa { get; set; } = default!;

        public OrigenDocumentoCompra Origen { get; set; } = OrigenDocumentoCompra.Contabilidad;
        public ProcedenciaDocumentoCompra? Procedencia { get; set; }
        public EstadoDocumentoCompra Estado { get; set; } = EstadoDocumentoCompra.Pendiente;

        // Detracciones
        public string? TipoBienCode { get; set; }
        public TipoBien? TipoBien { get; set; }

        public string? TipoOperacionCode { get; set; }
        public TipoOperacion? TipoOperacion { get; set; }

        public string? OperacionDetraccion { get; set; }
        public string? ConstanciaDetraccion { get; set; }
        public DateTime? FechaConstanciaDetraccion { get; set; }
        public string? NumeroDetraccion { get; set; }
        public DateTime? FechaEmisionDetraccion { get; set; }
        public decimal MontoDetraccionSol { get; set; }

        // Documento origen (nota de crédito -> factura)
        public string? DocumentoOrigenComprobantePagoCode { get; set; }
        public string? DocumentoOrigenNumero { get; set; }
        public DocumentoCompra? DocumentoOrigen { get; set; }

        public string? OrdenCompraAdministracionCode { get; set; }
        public OrdenCompraAdministracion? OrdenCompraAdministracion { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public decimal PagadoSol { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        /// <summary>Columna calculada en BD (TotalSol - PagadoSol).</summary>
        public decimal PendienteSol { get; private set; }

        /// <summary>Código del asiento de provisión (se genera en un proceso posterior; sin FK).</summary>
        public string? AsientoProvisionCode { get; set; }

        public ICollection<DocumentoCompraDetalle> Detalles { get; set; } = new List<DocumentoCompraDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
