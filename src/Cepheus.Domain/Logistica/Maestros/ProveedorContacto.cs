using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Contacto de un proveedor (tabla de detalle, N contactos por proveedor).
    ///
    /// Legacy: dbo.MProveedorContactos (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   IdContacto     -> Id (PK real, IDENTITY)
    ///   Codigo_prv     -> ProveedorCode (FK -> Proveedor.Code)
    ///   Nombres        -> FirstName
    ///   Apellidos      -> LastName
    ///   Cargo          -> Position
    ///   Telefono       -> Phone
    ///   TelefonoMovil  -> MobilePhone
    ///   Correo         -> Email
    ///   EsPrincipal    -> IsPrimary (solo un contacto principal por
    ///                     proveedor; exclusividad manejada en el handler,
    ///                     mismo criterio que ProveedorDireccion)
    ///   Estado         -> reemplazado por IsActive estándar
    /// </summary>
    public class ProveedorContacto : IAuditableEntity
    {
        public int Id { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string FirstName { get; set; } = default!;
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? MobilePhone { get; set; }
        public string? Email { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
