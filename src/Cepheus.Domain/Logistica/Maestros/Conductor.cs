using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Conductor (persona que maneja los vehículos de transporte). Entidad
    /// maestra del módulo de Logística.
    ///
    /// Legacy: dbo.MConductores (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   CodigoConductor    -> Code (PK natural, char(5), correlativo)
    ///   TipoDocumento      -> DocumentTypeCode (FK -> Comunes.TipoDocumento.Code,
    ///                         mismo criterio ya confirmado en Proveedor/Transportista)
    ///   NumeroDocumento    -> DocumentNumber
    ///   Nombres            -> FirstName
    ///   Apellidos          -> LastName
    ///   LicenciaConducir   -> DriverLicenseNumber (único)
    ///   CategoriaLicencia  -> LicenseCategory (texto libre — sin catálogo,
    ///                         dominio abierto: A-I, A-IIa, A-IIIc, etc.)
    ///   Telefono           -> Phone
    ///   Correo             -> Email
    ///   Estado             -> reemplazado por IsActive estándar
    ///   Observaciones      -> Observations
    ///
    /// El legacy define UNIQUE(TipoDocumento, NumeroDocumento) y
    /// UNIQUE(LicenciaConducir) — ambas se replican.
    /// </summary>
    public class Conductor : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string DocumentTypeCode { get; set; } = default!;
        public Comunes.TipoDocumento DocumentType { get; set; } = default!;

        public string DocumentNumber { get; set; } = default!;

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public string DriverLicenseNumber { get; set; } = default!;
        public string? LicenseCategory { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }

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
