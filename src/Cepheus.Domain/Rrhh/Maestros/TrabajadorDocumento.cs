using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Documento de identidad de un trabajador (tabla de detalle, N documentos
    /// por trabajador — ej. DNI y Carné de Extranjería).
    ///
    /// Fuente: script RRHH provisto (rrhh.trabajador_documento). Sin columna de
    /// estado en el script -> no expone IsActive/Toggle; se elimina el registro
    /// (ver DeleteTrabajadorDocumento) en vez de desactivarlo.
    ///
    /// Mapeo columnas script -> propiedades profesionales:
    ///   id                -> Id (PK real, IDENTITY)
    ///   trabajador_id     -> TrabajadorCode (FK -> Trabajador.Code)
    ///   tipo_documento_id -> TipoDocumentoCode (FK -> TipoDocumentoIdentidad.Code)
    ///   numero_documento  -> DocumentNumber
    ///   principal         -> IsPrimary (un solo documento principal por
    ///                        trabajador; exclusividad manejada en el handler,
    ///                        mismo criterio que Logistica.ProveedorCuenta)
    ///
    /// uq_trabajador_documento UNIQUE(tipo_documento_id, numero_documento) ->
    /// índice único compuesto (TipoDocumentoCode, DocumentNumber).
    /// </summary>
    public class TrabajadorDocumento : IAuditableEntity
    {
        public int Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public string TipoDocumentoCode { get; set; } = default!;
        public TipoDocumento TipoDocumento { get; set; } = default!;

        public string DocumentNumber { get; set; } = default!;

        public bool IsPrimary { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}