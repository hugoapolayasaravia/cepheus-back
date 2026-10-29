using Cepheus.Domain.Administracion;
using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Vendedor. Maestro del módulo de Facturación y Ventas, relacionado con el
    /// usuario del sistema.
    ///
    /// Legacy: dbo.Mvendedores (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_ven      -> Code (PK natural, char(4), correlativo automático)
    ///   Nombre_ven      -> Name (varchar(50) -> 100)
    ///   Abreviatura_ven -> Abbreviation (opcional)
    ///   Direccion_ven   -> Address (50 -> 200; opcional)
    ///   Telefono_ven    -> Phone (25 -> 30; opcional)
    ///   Correo_ven      -> Email (50 -> 150; opcional)
    ///   Titulo_ven      -> Title (nvarchar(10); opcional)
    ///   codigo_est      -> reemplazado por IsActive (ver análisis de TEstados)
    ///   Login_usu       -> UserId (FK -> Administracion.User; era un login libre
    ///                      varchar(20), NULL permitido; opcional y único: un usuario
    ///                      no puede ser dos vendedores)
    ///
    /// Los NOT NULL de dirección, teléfono, correo y título en el legacy se relajan
    /// a opcionales: los blancos se migran como NULL.
    /// </summary>
    public class Vendedor : IAuditableEntity
    {
        /// <summary>Código del vendedor (PK natural, 4 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Abbreviation { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Title { get; set; }

        /// <summary>Usuario del sistema asociado (opcional; único por vendedor).</summary>
        public int? UserId { get; set; }

        public User? User { get; set; }

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
