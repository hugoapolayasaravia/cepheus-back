using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Cotización: documento raíz del proceso comercial de Facturación
    /// (cabecera + detalle de productos + notas + metrado). Aggregate root
    /// transaccional: sus hijos (CotizacionDetalle, CotizacionNota,
    /// CotizacionMetradoResumen, CotizacionMetradoDetalle) solo existen a
    /// través de una Cotizacion y se gestionan con operaciones del documento
    /// (crear, agregar línea, aprobar, anular), no con CRUD de catálogo — por
    /// eso esta entrega solo incluye el modelo de datos (Domain + EF Config);
    /// los Commands transaccionales quedan para una siguiente entrega, una
    /// vez validado el flujo de negocio (numeración, aprobación, anulación).
    ///
    /// Legacy: dbo.CotizacionResumen (SQL Server). PK compuesta
    /// (Codigo_Neg, Ano_cot, Mes_cot, Codigo_cot): se mantiene como PK
    /// compuesta, mismo criterio que el resto de correlativos por negocio/año
    /// del sistema legacy.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales (resumen; ver
    /// también CotizacionDetalle, CotizacionNota, CotizacionMetradoResumen y
    /// CotizacionMetradoDetalle):
    ///   Codigo_Neg, Ano_cot, Mes_cot, Codigo_cot -> NegocioCode, Year, Month,
    ///                        Code (PK compuesta)
    ///   Codigo_ven           -> VendedorCode (FK -> Vendedor)
    ///   Fecha_cot            -> Date
    ///   Moneda_cot           -> CurrencyCode (char(1); PENDIENTE CONFIRMAR
    ///                           equivalencia con Comunes.Moneda.Code (ISO
    ///                           alfabético), mismo caso que Cliente/Obra)
    ///   codigo_pgv           -> FormaPagoVentaCode (FK -> FormaPagoVenta)
    ///   Codigo_Tec           -> TecnicoCode (FK -> Tecnico, opcional)
    ///   Igv_Met              -> AppliesIgv (char(1) SI/NO -> bool)
    ///   Descuento_cot        -> Discount
    ///   Volumen_glo          -> GlobalVolume
    ///   Ind_Modificable      -> IsEditable (char(1) S/N -> bool)
    ///   Tipo_Cot             -> Type (enum TipoCotizacion)
    ///   Duracion_Obr         -> WorkDurationMonths
    ///   Codigo_cli           -> ClienteCode (FK -> Cliente, opcional: puede
    ///                           cotizarse sin cliente registrado)
    ///   Ruc_cot              -> Ruc (cuando el cliente no está registrado)
    ///   Cliente_cot          -> ClientName
    ///   DireccionC_cot       -> ClientAddress
    ///   UbiDireccion_Cli     -> ClientAddressUbigeoCode (FK -> Ubigeo)
    ///   Codigo_obr           -> ObraCode (FK compuesta opcional junto con
    ///                           ClienteCode -> Obra)
    ///   Obra_cot             -> WorkName
    ///   Estado_Pry           -> ProjectStatus (enum EstadoProyectoCotizacion)
    ///   UbiDireccion_Obr     -> WorkAddressUbigeoCode (FK -> Ubigeo)
    ///   Direccion_cot        -> WorkAddress
    ///   Contacto_cot, Telefono_cot, emaicont_cot, referencia_cot ->
    ///                           ContactName, ContactPhone, ContactEmail, Reference
    ///   FechaIni_cot, FechaFin_cot -> StartDate, EndDate
    ///   fecestdes_cot        -> DispatchDate
    ///   Codigo_fle           -> FleteCode (FK -> Flete)
    ///   tasaigv_cot          -> IgvRate (snapshot de la tasa de IGV vigente
    ///                           al emitir; no se relaciona con
    ///                           Comunes.ControlVentas.IgvPercentage porque un
    ///                           documento ya emitido no debe cambiar si la
    ///                           tasa global cambia después)
    ///   Fecproceso_cot       -> ProcessDate
    ///   Codigo_usu           -> ELIMINADO como columna propia: se cubre con
    ///                           CreatedBy (IAuditableEntity), mismo criterio
    ///                           que VehiculoVenta.codigo_usu/fecha_pro.
    ///   Bruto_cot, Igv_cot, Neto_cot -> GrossAmount, IgvAmount, NetAmount
    ///   Estado_cot           -> Status (enum EstadoCotizacion)
    ///   Negocio_cot, AnoOriginal_cot, MesOriginal_cot, Original_cot ->
    ///                           OriginNegocioCode, OriginYear, OriginMonth,
    ///                           OriginCode (auto-FK compuesta opcional: origen
    ///                           cuando la cotización es copia/recotización)
    ///   Login_apr, Fecha_apr -> ApprovedBy, ApprovedAt (nullable; en el
    ///                           legacy Login_apr era NOT NULL aunque solo se
    ///                           llena al aprobar — se corrige a nullable)
    ///   Motivo_Anu, Login_Anu, Fecha_Anu -> CancelReason, CanceledBy, CanceledAt
    ///   flg_impresion        -> IsPrinted
    ///   Observaciones_Cot    -> Observations
    /// </summary>
    public class Cotizacion : IAuditableEntity
    {
        public string NegocioCode { get; set; } = default!;
        public Negocio Negocio { get; set; } = default!;

        /// <summary>Año de la cotización (char(4) en el legacy).</summary>
        public string Year { get; set; } = default!;

        /// <summary>Mes de la cotización (char(2) en el legacy).</summary>
        public string Month { get; set; } = default!;

        /// <summary>Correlativo por año (char(8) en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string VendedorCode { get; set; } = default!;
        public Vendedor Vendedor { get; set; } = default!;

        public DateTime Date { get; set; }

        /// <summary>Ver nota PENDIENTE CONFIRMAR en el comentario de la clase.</summary>
        public string CurrencyCode { get; set; } = default!;

        public string FormaPagoVentaCode { get; set; } = default!;
        public FormaPagoVenta FormaPagoVenta { get; set; } = default!;

        public string? TecnicoCode { get; set; }
        public Tecnico? Tecnico { get; set; }

        public bool AppliesIgv { get; set; } = true;
        public decimal Discount { get; set; }
        public decimal GlobalVolume { get; set; }
        public bool IsEditable { get; set; } = true;

        public TipoCotizacion Type { get; set; } = TipoCotizacion.Cotizacion;

        /// <summary>
        /// Motor de fórmulas de metrado (ver SistemaCalculoMetrado). Legacy:
        /// Flag_Tipo — columna distinta de Tipo_Cot, ver nota en el enum.
        /// Solo obligatorio en el legacy para el negocio 'PT'; nullable acá
        /// porque el resto de negocios no usa metrado de bovedillas.
        /// </summary>
        public SistemaCalculoMetrado? MetradoCalculationSystem { get; set; }

        public int WorkDurationMonths { get; set; }

        public string? ClienteCode { get; set; }
        public Cliente? Cliente { get; set; }

        public string? Ruc { get; set; }
        public string ClientName { get; set; } = default!;
        public string? ClientAddress { get; set; }

        public string ClientAddressUbigeoCode { get; set; } = default!;
        public Ubigeo ClientAddressUbigeo { get; set; } = default!;

        public string? ObraCode { get; set; }
        public Obra? Obra { get; set; }

        public string WorkName { get; set; } = default!;

        public EstadoProyectoCotizacion ProjectStatus { get; set; }

        public string WorkAddressUbigeoCode { get; set; } = default!;
        public Ubigeo WorkAddressUbigeo { get; set; } = default!;

        public string WorkAddress { get; set; } = default!;

        public string ContactName { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
        public string? Reference { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DispatchDate { get; set; }

        public string FleteCode { get; set; } = default!;
        public Flete Flete { get; set; } = default!;

        public decimal IgvRate { get; set; }

        public DateTime ProcessDate { get; set; }

        public decimal GrossAmount { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal NetAmount { get; set; }

        public EstadoCotizacion Status { get; set; } = EstadoCotizacion.Pendiente;

        public string? OriginNegocioCode { get; set; }
        public string? OriginYear { get; set; }
        public string? OriginMonth { get; set; }
        public string? OriginCode { get; set; }
        public Cotizacion? Origin { get; set; }

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public string? CancelReason { get; set; }
        public string? CanceledBy { get; set; }
        public DateTime? CanceledAt { get; set; }

        public bool IsPrinted { get; set; }

        public string? Observations { get; set; }

        // Hijos del aggregate
        public ICollection<CotizacionDetalle> Detalles { get; set; } = new List<CotizacionDetalle>();
        public ICollection<CotizacionNota> Notas { get; set; } = new List<CotizacionNota>();
        public ICollection<CotizacionMetradoResumen> MetradoResumenes { get; set; } = new List<CotizacionMetradoResumen>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
