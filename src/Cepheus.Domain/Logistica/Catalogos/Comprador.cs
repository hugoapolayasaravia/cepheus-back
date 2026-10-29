using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Comprador responsable de las Órdenes de Compra (catálogo simple). Usado
    /// en el módulo de Logística.
    ///
    /// Legacy: dbo.TCompradores (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_cop  -> Code (PK natural, char(3))
    ///   Nombre_cop  -> Name
    ///   codigo_est  -> reemplazado por IsActive (misma decisión que en
    ///                  LugarEnvio: no había tabla de estados en el legacy)
    /// </summary>
    public class Comprador : IAuditableEntity
    {
        /// <summary>
        /// Código del comprador (PK natural, 3 caracteres).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

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