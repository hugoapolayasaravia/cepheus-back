using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Rrhh.Catalogos;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Domicilio de un trabajador (tabla de detalle, N domicilios por
    /// trabajador).
    ///
    /// Fuente: script RRHH provisto (rrhh.trabajador_domicilio). Sin columna de
    /// estado -> no expone IsActive/Toggle; se elimina el registro (ver
    /// DeleteTrabajadorDomicilio) en vez de desactivarlo.
    ///
    /// Mapeo columnas script -> propiedades profesionales:
    ///   id             -> Id (PK real, IDENTITY)
    ///   trabajador_id  -> TrabajadorCode (FK -> Trabajador.Code)
    ///   tipo_via_id    -> RoadTypeCode (FK -> TipoVia.Code)
    ///   nombre_via     -> StreetName
    ///   numero_via     -> StreetNumber
    ///   interior       -> InteriorNumber
    ///   tipo_zona_id   -> ZoneTypeCode (FK -> TipoZona.Code)
    ///   nombre_zona    -> ZoneName
    ///   referencia     -> Reference
    ///   ubigeo_id      -> UbigeoCode (FK -> comun.Ubigeo.Code — la FK que el
    ///                      script dejaba comentada; se conecta porque
    ///                      Comunes.Ubigeo ya existe en el repo)
    ///   principal      -> IsPrimary (un solo domicilio principal por
    ///                      trabajador; exclusividad manejada en el handler,
    ///                      mismo criterio que TrabajadorDocumento).
    ///                      Nota: en el script el default de esta columna es 1
    ///                      (a diferencia de trabajador_documento.principal,
    ///                      cuyo default es 0).
    /// </summary>
    public class TrabajadorDomicilio : IAuditableEntity
    {
        public int Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public string? RoadTypeCode { get; set; }
        public TipoVia? RoadType { get; set; }

        public string? StreetName { get; set; }
        public string? StreetNumber { get; set; }
        public string? InteriorNumber { get; set; }

        public string? ZoneTypeCode { get; set; }
        public TipoZona? ZoneType { get; set; }

        public string? ZoneName { get; set; }

        public string? Reference { get; set; }

        public string? UbigeoCode { get; set; }
        public Ubigeo? Ubigeo { get; set; }

        public bool IsPrimary { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}