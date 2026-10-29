using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Nota de cotización reutilizable (plantilla), agrupada por negocio y por
    /// tipo (Observación / Consideraciones / Transporte). Se usa como texto
    /// base al agregar notas a una cotización (Facturacion.Transacciones.
    /// CotizacionNota); no confundir con esta última, que es la nota concreta
    /// dentro de una cotización ya emitida.
    ///
    /// Legacy: dbo.CotizacionTNotas (SQL Server). PK compuesta
    /// (Codigo_Neg, Correlativo_not): se mantiene como PK compuesta, mismo
    /// criterio que Obra (correlativo por negocio, no código único global).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Neg      -> NegocioCode (FK -> Comun.Negocio.Code, parte de la PK)
    ///   Correlativo_not -> Code (correlativo por negocio, char(2), parte de la PK)
    ///   Descripcion_not -> Description (text -> string, sin límite corto)
    ///   Estado_not      -> IsActive (char(2), Activo/Inactivo) -> reemplazado
    ///                      por bool, mismo criterio que el resto de catálogos.
    ///   Opcion_not      -> Option (enum OpcionNotaCotizacion, default 'O'
    ///                      Observación)
    /// </summary>
    public class NotaCotizacionPlantilla : IAuditableEntity
    {
        public string NegocioCode { get; set; } = default!;
        public Negocio Negocio { get; set; } = default!;

        /// <summary>Correlativo por negocio (char(2) en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string Description { get; set; } = default!;

        public OpcionNotaCotizacion Option { get; set; } = OpcionNotaCotizacion.Observacion;

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
