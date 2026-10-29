using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Lugar de envío/entrega para Órdenes de Compra (catálogo simple). Usado
    /// en el módulo de Logística.
    ///
    /// Legacy: dbo.TLugarEnvio (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_env       -> Code (PK natural, char(3))
    ///   Descripcion_env  -> Name
    ///   Direccion_env    -> Address (opcional)
    ///   Codigo_est       -> reemplazado por IsActive (decisión del negocio:
    ///                        no había tabla de estados en el legacy, se
    ///                        descarta el campo y se usa el booleano estándar
    ///                        del resto de catálogos)
    /// </summary>
    public class LugarEnvio : IAuditableEntity
    {
        /// <summary>
        /// Código del lugar de envío (PK natural, 3 caracteres).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Address { get; set; }

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