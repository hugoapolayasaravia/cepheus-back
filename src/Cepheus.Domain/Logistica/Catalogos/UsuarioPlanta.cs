using Cepheus.Domain.Administracion;
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Asignación de un usuario del sistema a una planta. Tabla de enlace
    /// (muchos a muchos) del catálogo de Logística: un usuario puede operar
    /// en varias plantas y una planta tiene varios usuarios.
    ///
    /// Legacy: dbo.TUsuarioXPlanta (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Login_apr   -> UserId (FK -> Administracion.User; era un login libre
    ///                  varchar(20), mismo criterio que Vendedor.UserId)
    ///   Codigo_Pla  -> PlantaCode (FK -> Comunes.Planta, char(2))
    ///
    /// PK compuesta (UserId, PlantaCode), igual que el legacy
    /// PK_TUsuarioXPlanta (Login_apr, Codigo_Pla). No tiene IsActive: la
    /// asignación existe o no existe (se crea / se elimina).
    /// </summary>
    public class UsuarioPlanta : IAuditableEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
