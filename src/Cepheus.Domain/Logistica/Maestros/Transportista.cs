using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Transportista (empresa o persona que presta el servicio de
    /// transporte). Entidad maestra del módulo de Logística.
    ///
    /// Legacy: dbo.MTransportistas (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   CodigoTransportista -> Code (PK natural, char(5), correlativo)
    ///   TipoDocumento       -> DocumentTypeCode (FK -> Comunes.TipoDocumento.Code,
    ///                          mismo criterio ya confirmado en Proveedor)
    ///   NumeroDocumento     -> DocumentNumber
    ///   RazonSocial         -> LegalName
    ///   NombreComercial     -> TradeName
    ///   DireccionFiscal     -> Address
    ///   CodigoUbigeo        -> UbigeoCode (FK -> Comunes.Ubigeo.Code)
    ///   Telefono            -> Phone
    ///   Correo              -> Email
    ///   RegistroMTC         -> MtcRegistrationNumber (texto libre, sin catálogo
    ///                          — registro ante el Ministerio de Transportes)
    ///   EsPropio            -> IsOwnFleet
    ///   Estado              -> reemplazado por IsActive estándar
    ///   Observaciones       -> Observations
    ///   CertificationNumber -> Número de certificación del transportista.
    ///
    /// El legacy define UNIQUE(TipoDocumento, NumeroDocumento) — se replica
    /// como índice único compuesto. Además se valida unicidad de RazonSocial,
    /// extendiendo por consistencia la misma regla de negocio pedida para
    /// Proveedores (RUC/Nombre no repetidos) — no estaba pedida explícitamente
    /// para Transportistas, avisar si no corresponde.
    /// </summary>
    public class Transportista : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string DocumentTypeCode { get; set; } = default!;
        public TipoDocumento DocumentType { get; set; } = default!;

        public string DocumentNumber { get; set; } = default!;

        public string LegalName { get; set; } = default!;
        public string? TradeName { get; set; }

        public string? Address { get; set; }
        public string? UbigeoCode { get; set; }
        public Ubigeo? Ubigeo { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string? MtcRegistrationNumber { get; set; }
        public string? CertificationNumber { get; set; }

        public bool IsOwnFleet { get; set; }

        public string? Observations { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
