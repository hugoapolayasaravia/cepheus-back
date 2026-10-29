using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Técnico: trabajador de RRHH habilitado para ser asignado como técnico
    /// responsable de una cotización (Cotizacion.TecnicoCode). Entidad maestra
    /// del módulo de Facturación y Ventas, PK 1:1 con Rrhh.Maestros.Trabajador.
    ///
    /// Legacy: dbo.TTecnicos (SQL Server). Comentario del DDL: "relacionalo
    /// con la tabla trabajadores de RRHH".
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_tra   -> TrabajadorCode (PK natural, FK -> Rrhh.Maestros.Trabajador.Code,
    ///                   char(5); la tabla no agrega datos propios del técnico,
    ///                   solo habilita/deshabilita a un Trabajador existente
    ///                   como técnico para Facturación)
    ///   codigo_est   -> IsActive (char(2), FK TEstados) -> reemplazado por
    ///                   bool, mismo criterio que Flete/Vendedor/Cobrador.
    /// </summary>
    public class Tecnico : IAuditableEntity
    {
        /// <summary>Código del trabajador de RRHH habilitado como técnico (PK, FK 1:1).</summary>
        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

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
