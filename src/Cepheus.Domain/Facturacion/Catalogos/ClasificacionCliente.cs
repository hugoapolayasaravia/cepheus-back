using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Clasificación de cliente (catálogo simple). Usado en el módulo de Facturación
    /// para clasificar a los clientes (Cliente.ClasificacionCode), de forma
    /// independiente del Tipo de Cliente.
    ///
    /// Legacy: no se entregó el DDL de esta tabla. Solo se conoce la columna
    /// dbo.MClientes.Codigo_cla char(1) NOT NULL (comentario: "Clasificación"), sin
    /// FK declarada en el script. Se modela con la misma forma que TTipoCliente.
    /// Mapeo (supuesto):
    ///   Codigo_cla  -> Code (PK natural, char(1))
    ///   (nombre)    -> Name (longitud asumida: 30)
    ///
    /// IsActive no existe en el legacy; se agrega por consistencia con el resto
    /// de catálogos del sistema.
    /// </summary>
    public class ClasificacionCliente : IAuditableEntity
    {
        /// <summary>
        /// Código de la clasificación (PK natural, 1 carácter).
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
