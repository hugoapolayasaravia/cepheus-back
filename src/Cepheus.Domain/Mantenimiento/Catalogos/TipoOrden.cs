using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Catalogos
{
    /// <summary>
    /// Tipo de Orden de Trabajo (ej. "MCR" = Mantenimiento Correctivo).
    /// Catálogo del módulo Mantenimiento.
    ///
    /// Legacy: dbo.TTipoOrdenes (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_TOT       -> Code (PK natural, char(3), código mnemotécnico
    ///                       asignado manualmente — mismo criterio que
    ///                       Inspeccion.Code)
    ///   Descripcion_TOT  -> Name
    ///   Codigo_Est       -> ELIMINADO. En el legacy traía un default fijo
    ///                       ('05' = Pendiente) que en realidad es el estado
    ///                       INICIAL de la Orden de Trabajo al crearse, no un
    ///                       atributo propio del tipo de orden. Con
    ///                       EstadoOrdenTrabajo como enum (ver módulo
    ///                       Transacciones), toda OT nueva simplemente nace
    ///                       en EstadoOrdenTrabajo.Pendiente — no hace falta
    ///                       guardarlo acá.
    /// </summary>
    public class TipoOrden : IAuditableEntity
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
