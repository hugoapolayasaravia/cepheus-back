using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Rrhh.Catalogos;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Trabajador. Entidad maestra del módulo RRHH.
    ///
    /// Fuente: script RRHH provisto (rrhh.trabajador).
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo               -> Code (PK natural, char(5), autogenerado
    ///                            correlativamente por la aplicación — mismo
    ///                            patrón que Logistica.Maestros.Proveedor)
    ///   nombres              -> FirstNames
    ///   apellido_paterno     -> PaternalSurname
    ///   apellido_materno     -> MaternalSurname
    ///   sexo_id              -> SexoCode (FK -> rrhh.Sexo.Code)
    ///   estado_civil_id      -> EstadoCivilCode (FK -> rrhh.EstadoCivil.Code)
    ///   nacionalidad_id      -> NacionalidadCode (FK -> rrhh.Nacionalidad.Code)
    ///   fecha_nacimiento     -> BirthDate
    ///   ubigeo_nacimiento_id -> BirthUbigeoCode (FK -> comun.Ubigeo.Code — la FK
    ///                            que el script dejaba comentada; se conecta
    ///                            porque Comunes.Ubigeo ya existe en el repo)
    ///   email                -> Email
    ///   telefono             -> Phone
    ///   celular              -> MobilePhone
    ///   foto                 -> PhotoUrl
    ///   discapacidad         -> HasDisability
    ///
    /// La tabla no tiene columna "activo": el estado laboral del trabajador
    /// (activo/cesado/licencia) se gestiona en rrhh.trabajador_laboral
    /// (estado_trabajador_id + fecha_cese), no en este maestro. Por eso esta
    /// entidad no expone IsActive ni Toggle.
    /// </summary>
    public class Trabajador : IAuditableEntity
    {
        /// <summary>
        /// Código del trabajador (PK natural, 5 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public string? FirstNames { get; set; }
        public string? PaternalSurname { get; set; }
        public string? MaternalSurname { get; set; }

        public string? SexoCode { get; set; }
        public Sexo? Sexo { get; set; }

        public string? EstadoCivilCode { get; set; }
        public EstadoCivil? EstadoCivil { get; set; }

        public string? NacionalidadCode { get; set; }
        public Nacionalidad? Nacionalidad { get; set; }

        public DateOnly? BirthDate { get; set; }

        public string? BirthUbigeoCode { get; set; }
        public Ubigeo? BirthUbigeo { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? MobilePhone { get; set; }

        public string? PhotoUrl { get; set; }

        public bool HasDisability { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}