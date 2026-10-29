using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Tipo de Hoja de Corte. Catálogo del módulo de Facturación y Ventas.
    ///
    /// Legacy: dbo.TTipoHojaCorte (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_thc      -> Code (PK natural, char(2))
    ///   descripcion_thc -> Name (varchar(30))
    ///   (sin estado en el legacy) -> IsActive, estándar de los catálogos
    ///
    /// Valores del legacy (datos, ver sql/TipoHojaCorte_seed.sql):
    ///   01 NUEVO | 03 ADICIONAL | 04 REPOSICION | 05 REEMPLAZO
    ///
    /// IMPORTANTE: el código de este catálogo gobierna reglas del documento
    /// (03 y 05 exigen Hoja de Corte padre; 05 exige motivo). Esos códigos
    /// están en TipoHojaCorteCodes. Como los códigos legacy no son
    /// correlativos (falta el 02), los cuatro valores se cargan con el script
    /// de seed y NO se crean por el endpoint (que autogenera el siguiente
    /// correlativo, igual que el resto de catálogos).
    /// </summary>
    public class TipoHojaCorte : IAuditableEntity
    {
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
