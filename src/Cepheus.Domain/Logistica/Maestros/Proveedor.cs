using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Proveedor de compras. Entidad maestra del módulo de Logística.
    ///
    /// Legacy: dbo.MProveedores (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_prv       -> Code (PK natural, char(5), autogenerado
    ///                        correlativamente por la aplicación)
    ///   TipoDocumento    -> DocumentTypeCode (FK -> Comunes.TipoDocumento.Code).
    ///                        NOTA: se reutiliza el catálogo Comunes.TipoDocumento
    ///                        por decisión explícita, aunque esa tabla está
    ///                        documentada como clasificación TRIBUTARIA
    ///                        (IGV/Renta) y no como tipo de documento de
    ///                        identidad (RUC/DNI/CE). Si al cargar datos reales
    ///                        el catálogo no calza semánticamente, avisar para
    ///                        separar en un catálogo de identidad aparte.
    ///   NumeroDocumento  -> DocumentNumber (único)
    ///   RazonSocial      -> LegalName (único)
    ///   NombreComercial  -> TradeName
    ///   TipoProveedor    -> ProviderType (enum: Juridica/Natural, antes 'J'/'N')
    ///   Procedencia      -> Origin (enum: Nacional/Extranjero)
    ///   Estado           -> reemplazado por IsActive estándar (decisión: los
    ///                        estados Inactivo/Suspendido/Bloqueado del legacy
    ///                        colapsan todos a IsActive = false)
    ///   CondicionSunat   -> SunatCondition (enum, nullable)
    ///   EstadoSunat      -> SunatTaxpayerStatus (enum, nullable)
    ///   Observaciones    -> Observations
    ///   enlace1_cta / enlace2_cta -> tabla ProveedorCuentaContable (una cuenta por pagar 42/46 por proveedor y
    ///                        moneda, sin límite de monedas). La usa la provisión contable de compras (asiento tipo 02).
    ///   FechaCreacion,
    ///   UsuarioCreacion,
    ///   FechaModificacion,
    ///   UsuarioModificacion -> auditoría estándar (IAuditableEntity)
    ///   FechaBaja,
    ///   UsuarioBaja      -> DeactivatedAt / DeactivatedBy (se completan cuando
    ///                        IsActive pasa a false; se limpian si se reactiva)
    /// </summary>
    public class Proveedor : IAuditableEntity
    {
        /// <summary>
        /// Código del proveedor (PK natural, 5 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public string DocumentTypeCode { get; set; } = default!;
        public TipoDocumento DocumentType { get; set; } = default!;

        public string DocumentNumber { get; set; } = default!;

        public string LegalName { get; set; } = default!;
        public string? TradeName { get; set; }

        public ProviderType ProviderType { get; set; }
        public ProviderOrigin Origin { get; set; }

        public SunatCondition? SunatCondition { get; set; }
        public SunatTaxpayerStatus? SunatStatus { get; set; }

        public string? Observations { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Cuándo se dio de baja (IsActive = false). Null si nunca se dio de baja
        /// o si está actualmente activo.
        /// </summary>
        public DateTime? DeactivatedAt { get; set; }
        public string? DeactivatedBy { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
