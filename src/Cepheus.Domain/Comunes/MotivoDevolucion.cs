using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Motivo de devolución (ej. Producto defectuoso, Error de pedido, Cambio de
    /// producto). Catálogo maestro compartido entre módulos (Ventas, Logística,
    /// Inventario) para clasificar devoluciones y su efecto sobre el stock.
    ///
    /// Legacy: dbo.Tmotivos (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_mot        -> Code
    ///   descripcion_mot   -> Name
    ///   afecta_mot        -> AffectsStock ('S'/'N' -> bool)
    ///
    /// Catálogo simple, sin ambigüedades: el nombre original de la tabla y sus
    /// columnas ya coinciden 1:1 con "motivo de devolución" (afecta_mot indica
    /// si ese motivo genera movimiento de inventario al aplicarse, ej. una
    /// devolución por "producto defectuoso" sí afecta stock, mientras que un
    /// motivo puramente administrativo podría no hacerlo).
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con el
    /// resto de catálogos del sistema (mismo caso que TipoDocumento/Ubigeo).
    /// </summary>
    public class MotivoDevolucion : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool AffectsStock { get; set; }
        public bool EsVenta { get; set; }

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