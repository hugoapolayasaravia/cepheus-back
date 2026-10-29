using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Cliente. Entidad maestra principal del módulo de Facturación.
    ///
    /// Legacy: dbo.MClientes (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_cli           -> Code (PK natural, char(5), correlativo)
    ///   Tipo_cli             -> PersonType (enum TipoPersona, antes 'E'/'N')
    ///   Codigo_tdo           -> DocumentTypeCode (FK -> Comunes.TipoDocumento.Code).
    ///                           NOTA: mismo criterio ya confirmado en
    ///                           Proveedor/Conductor/Transportista — se reutiliza
    ///                           Comunes.TipoDocumento aunque esa tabla está
    ///                           documentada como clasificación TRIBUTARIA
    ///                           (IGV/Renta) y no como tipo de documento de
    ///                           identidad (RUC/DNI/CE). Se vuelve obligatorio
    ///                           (era nullable en el legacy), decisión aprobada.
    ///   NumeroDocumento_cli  -> DocumentNumber (único)
    ///   Nombre_cli           -> Name (nullable como el legacy; único cuando se
    ///                           informa, vía índice único filtrado)
    ///   Direccion_cli        -> Address
    ///   UbiDireccion_cli     -> UbigeoCode (FK -> Comunes.Ubigeo.Code)
    ///   Telefono_cli         -> Phone
    ///   Codigo_cli_sup       -> ParentClientCode (FK -> MClientes.Code, auto-
    ///                           referencia opcional; el legacy no documenta el
    ///                           propósito exacto — se asume cliente matriz/holding)
    ///   Codigo_tcl           -> TipoClienteCode (FK -> Facturacion.TipoCliente.Code)
    ///   Codigo_cla           -> ClasificacionClienteCode (FK ->
    ///                           Facturacion.ClasificacionCliente.Code)
    ///   RLegal_cli           -> LegalRepresentativeName
    ///   TelRLegal_cli        -> LegalRepresentativePhone
    ///   DniRLegal_cli        -> LegalRepresentativeDni
    ///   Contacto_cli         -> ContactName
    ///   TelContacto_cli      -> ContactPhone
    ///   email_contacto_cli   -> ContactEmail
    ///   Observaciones_cli    -> Observations (nullable acá; el legacy la exigía
    ///                           NOT NULL, lo que en la práctica obligaba a
    ///                           guardar cadena vacía cuando no aplicaba)
    ///   Flag_Global          -> IsVip ("Cliente VIP Si/No")
    ///   ApruebaContado       -> RequiresCashOnly ("Aprueba solo contado")
    ///   LineaGlobal_cli      -> HasGlobalCreditLine
    ///   MontoGlobal_cli      -> GlobalCreditAmount (decimal(18,2), nullable)
    ///   OCompra_Cli          -> RequiresPurchaseOrderApproval (nullable,
    ///                           "Requiere Aprobación O.C.")
    ///   Aprobacon_Ot         -> RequiresWorkOrderApproval (nullable,
    ///                           "Requiere Aprobación O.T.")
    ///   CodigoPgv_cli        -> FormaPagoVentaCode (FK ->
    ///                           Facturacion.FormaPagoVenta.Code)
    ///   Moneda_cli           -> CurrencyCode (FK -> Comunes.Moneda.Code,
    ///                           nullable). NOTA: el legacy guarda char(1)
    ///                           numérico (ej. '1'/'2') y Comunes.Moneda usa
    ///                           código ISO alfabético (PEN/USD); la migración
    ///                           de datos debe traducir un valor al otro —
    ///                           confirmar la equivalencia antes de migrar.
    ///   ind_cli_gerencia     -> RequiresManagementApproval (nullable)
    ///   Estado_cli           -> Estado (enum EstadoCliente)
    ///
    /// IsActive no se agrega aparte porque Estado (enum) ya cubre el ciclo de
    /// vida completo, igual que en Proveedor con IsActive simple pero acá con
    /// más de dos valores relevantes (Suspendido, PedidoBloqueado).
    /// </summary>
    public class Cliente : IAuditableEntity
    {
        /// <summary>
        /// Código del cliente (PK natural, 5 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public TipoPersona PersonType { get; set; }

        public string DocumentTypeCode { get; set; } = default!;
        public TipoDocumento DocumentType { get; set; } = default!;

        public string DocumentNumber { get; set; } = default!;

        public string? Name { get; set; }

        public string Address { get; set; } = default!;

        public string UbigeoCode { get; set; } = default!;
        public Ubigeo Ubigeo { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public string? ParentClientCode { get; set; }
        public Cliente? ParentClient { get; set; }

        public string TipoClienteCode { get; set; } = default!;
        public TipoCliente TipoCliente { get; set; } = default!;

        public string ClasificacionClienteCode { get; set; } = default!;
        public ClasificacionCliente ClasificacionCliente { get; set; } = default!;

        public string? LegalRepresentativeName { get; set; }
        public string? LegalRepresentativePhone { get; set; }
        public string? LegalRepresentativeDni { get; set; }

        public string ContactName { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;

        public string? Observations { get; set; }

        public bool IsVip { get; set; }
        public bool RequiresCashOnly { get; set; }

        public bool HasGlobalCreditLine { get; set; }
        public decimal? GlobalCreditAmount { get; set; }

        public bool? RequiresPurchaseOrderApproval { get; set; }
        public bool? RequiresWorkOrderApproval { get; set; }

        public string FormaPagoVentaCode { get; set; } = default!;
        public FormaPagoVenta FormaPagoVenta { get; set; } = default!;

        public string? CurrencyCode { get; set; }
        public Moneda? Currency { get; set; }

        public bool? RequiresManagementApproval { get; set; }

        public EstadoCliente Estado { get; set; } = EstadoCliente.Activo;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
