using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Tipo de cliente (catálogo simple). Usado en el módulo de Facturación para
    /// clasificar a los clientes (Cliente.TipoClienteCode).
    ///
    /// Legacy: dbo.TTipoCliente (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tcl  -> Code (PK natural, char(1))
    ///   Nombre_tcl  -> Name (varchar(20))
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con el
    /// resto de catálogos del sistema.
    /// </summary>
    public class TipoCliente : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de cliente (PK natural, 1 carácter).
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
