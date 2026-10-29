using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Chofer que conduce los vehículos de un transportista. Entidad maestra del
    /// módulo de Facturación y Ventas.
    ///
    /// NO confundir con Logistica.Maestros.Conductor: mismo concepto de negocio,
    /// pero tabla y datos distintos (schema "facturacion"). Se nombra Chofer
    /// siguiendo la tabla legacy.
    ///
    /// Legacy: dbo.Tchoferes (SQL Server). PK compuesta (codigo_cho, codigo_tra):
    /// se mantiene como PK compuesta (TransportistaCode, Code), mismo criterio que
    /// OrdenTrabajo/ArticuloProveedor. El código del chofer es correlativo POR
    /// transportista.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_tra    -> TransportistaCode (FK -> Facturacion.Transportista.Code)
    ///   codigo_cho    -> Code (char(4), correlativo por transportista)
    ///   nombre_cho    -> FullName (nvarchar(70) -> 100; es un solo campo, no se
    ///                    parte en nombres/apellidos porque el legacy no lo permite
    ///                    de forma confiable)
    ///   brevete_cho   -> DriverLicenseNumber (nvarchar(20); el legacy usaba un
    ///                    espacio como default, ahora es obligatorio)
    ///   Observaciones -> Observations (varchar(500))
    ///
    /// Agregados frente al legacy: IsActive, auditoría (IAuditableEntity) y RowVersion.
    ///
    /// Sin índices únicos nuevos: el legacy solo tenía la PK, y el mismo brevete puede
    /// aparecer bajo varios transportistas (ver validaciones en
    /// docs/migracion/Chofer_validaciones.sql antes de decidir si conviene agregarlos).
    /// </summary>
    public class ChoferVenta : IAuditableEntity
    {
        public string TransportistaCode { get; set; } = default!;
        public TransportistaVenta TransportistaVenta { get; set; } = default!;

        /// <summary>Correlativo por transportista (codigo_cho, char(4) en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string FullName { get; set; } = default!;

        public string DriverLicenseNumber { get; set; } = default!;

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
