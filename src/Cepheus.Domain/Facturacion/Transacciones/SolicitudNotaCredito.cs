using Cepheus.Domain.Administracion;
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Solicitud de Nota de Crédito de Ventas: documento que se registra, se gestiona
    /// y se aprueba por etapas (Ventas -> Créditos Gestiona -> Créditos Aprueba ->
    /// Contabilidad -> Unidad de Negocio) antes de poder generar la Nota de Crédito.
    /// Aggregate root: sus hijos (Detalle, Guías de sustento, Archivos, Aprobaciones)
    /// solo existen a través de la solicitud.
    ///
    /// Legacy: dbo.RCreVentasSol. PK legacy (Codigo_snc, Ano_snc, Mes_snc) se mantiene.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_snc, Ano_snc, Mes_snc -> Code, Year, Month (PK compuesta)
    ///   Fecha_emi            -> IssueDate
    ///   Codigo_Cli           -> ClienteCode (FK -> Cliente)
    ///   Codigo_tdo           -> DocumentTypeCode (FK -> Comunes.ComprobantePago; tipo
    ///                           del comprobante a acreditar)
    ///   Numero_doc           -> DocumentNumber (número del comprobante a acreditar)
    ///   Numero_docF/Numero_docB -> ELIMINADOS: eran el mismo número partido por tipo
    ///                           para sostener FK a RFacVentas/RBolVentas. Esas tablas aún
    ///                           no existen en el modelo nuevo; cuando se migren, la
    ///                           validación se hace con (DocumentTypeCode, DocumentNumber).
    ///   Login_usu, Fecha_pro -> ELIMINADOS: se cubren con CreatedBy/CreatedAt
    ///   Codigo_est           -> Status (enum EstadoSolicitudNotaCredito)
    ///   Motivo_snc           -> MotivoCode (FK -> MotivoSolicitudNotaCredito)
    ///   Moneda_snc           -> MonedaCode (char(1) legacy -> FK Comunes.Moneda, ISO 4217)
    ///   Neto_snc, Igv_snc, Total_snc -> SubTotal, IgvAmount, TotalAmount
    ///   Codigo_neg           -> NegocioCode (FK -> Negocio)
    ///   Observacion_snc      -> Observation
    ///   flg_modifica         -> IsEditable
    ///   ind_procede_snc      -> IsProcedent (bool?: null = aún no evaluado)
    ///   COD_USUARIO_SOLICITA -> RequesterUserId (FK -> User; el legacy guardaba el login)
    ///   IND_ACEPTA_SOLICITANTE -> RequesterAccepts (bool?)
    ///
    /// Aprobaciones: el legacy tenía 5 grupos de columnas repetidas (Login_aprV/Fecha_aprV/
    /// Observa_V/IND_ESTADO_VTA, ..._C/_CRED, ..._G, ..._N/_NEGOCIO, ..._UC/_CONTABILIDAD).
    /// Se normalizan en SolicitudNotaCreditoAprobacion (una fila por etapa).
    /// SUPUESTO de correspondencia: V=Ventas, G=CreditosGestiona, C=CreditosAprueba,
    /// UC(+CONTABILIDAD)=Contabilidad, N=UnidadNegocio.
    /// </summary>
    public class SolicitudNotaCredito : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;

        public string NegocioCode { get; set; } = default!;
        public Negocio Negocio { get; set; } = default!;

        public DateTime IssueDate { get; set; }

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        public string DocumentTypeCode { get; set; } = default!;
        public ComprobantePago DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;

        public string MotivoCode { get; set; } = default!;
        public MotivoSolicitudNotaCredito Motivo { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public decimal SubTotal { get; set; }
        public decimal IgvAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string Observation { get; set; } = default!;

        public EstadoSolicitudNotaCredito Status { get; set; } = EstadoSolicitudNotaCredito.Pendiente;
        public bool IsEditable { get; set; } = true;
        public bool? IsProcedent { get; set; }

        public int? RequesterUserId { get; set; }
        public User? Requester { get; set; }
        public bool? RequesterAccepts { get; set; }

        public ICollection<SolicitudNotaCreditoDetalle> Details { get; set; } = new List<SolicitudNotaCreditoDetalle>();
        public ICollection<SolicitudNotaCreditoGuia> Guides { get; set; } = new List<SolicitudNotaCreditoGuia>();
        public ICollection<SolicitudNotaCreditoArchivo> Files { get; set; } = new List<SolicitudNotaCreditoArchivo>();
        public ICollection<SolicitudNotaCreditoAprobacion> Approvals { get; set; } = new List<SolicitudNotaCreditoAprobacion>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
