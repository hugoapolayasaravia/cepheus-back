using Cepheus.Domain.Administracion;
using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Cobrador. Maestro del módulo de Facturación y Ventas, relacionado con el
    /// usuario del sistema.
    ///
    /// Legacy: dbo.MCobradores (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_cob    -> Code (PK natural, char(4), correlativo automático)
    ///   Nombre_cob    -> Name (nvarchar(50) -> 100)
    ///   Direccion_cob -> Address (50 -> 200; opcional)
    ///   Telefono_cob  -> Phone (25 -> 30; opcional)
    ///   Correo        -> Email (50 -> 150; opcional)
    ///   Estado_cob    -> IsActive (char(1); a diferencia de Mvendedores, esta tabla
    ///                    no usaba TEstados)
    ///   login_mus     -> UserId (FK -> Administracion.User; era un login libre
    ///                    nvarchar(20), NULL permitido; opcional y único)
    /// </summary>
    public class Cobrador : IAuditableEntity
    {
        /// <summary>Código del cobrador (PK natural, 4 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        /// <summary>Usuario del sistema asociado (opcional; único por cobrador).</summary>
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
