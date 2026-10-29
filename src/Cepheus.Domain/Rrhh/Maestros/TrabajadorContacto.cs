using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Catalogos;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Contacto de emergencia/referencia de un trabajador (tabla de detalle,
    /// N contactos por trabajador).
    ///
    /// Fuente: script RRHH provisto (rrhh.trabajador_contacto). Sin columna de
    /// estado -> no expone IsActive/Toggle; se elimina el registro (ver
    /// DeleteTrabajadorContacto) en vez de desactivarlo.
    ///
    /// Mapeo columnas script -> propiedades profesionales:
    ///   id             -> Id (PK real, IDENTITY)
    ///   trabajador_id  -> TrabajadorCode (FK -> Trabajador.Code)
    ///   nombre         -> Name
    ///   telefono       -> Phone
    ///   parentesco_id  -> ParentescoCode (FK -> Parentesco.Code)
    ///   principal       -> IsPrimary (un solo contacto principal por
    ///                      trabajador; exclusividad manejada en el handler,
    ///                      mismo criterio que TrabajadorDocumento)
    /// </summary>
    public class TrabajadorContacto : IAuditableEntity
    {
        public int Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public string Name { get; set; } = default!;
        public string? Phone { get; set; }

        public string? ParentescoCode { get; set; }
        public Parentesco? Parentesco { get; set; }

        public bool IsPrimary { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}