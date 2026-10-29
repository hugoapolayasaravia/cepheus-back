using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Rrhh.Catalogos;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Información laboral del trabajador (fechas, área, cargo, régimen, etc.)
    ///
    /// Legacy: rrhh.trabajador_laboral (SQL Server).
    /// Relación con Trabajador: 1:1 — un único registro por trabajador (UNIQUE trabajador_id en el legacy).
    ///
    /// NOTA DE INTEGRACIÓN — pendiente de ajuste por el equipo:
    ///   - Se asume que la entidad Trabajador vive en
    ///     Cepheus.Domain.Rrhh.Trabajador con Id (long) como PK. Si tu
    ///     implementación usa otro namespace/tipo de PK, ajusta
    ///     TrabajadorId y la navegación.
    ///   - Se asume que los 45 catálogos de rrhh (Sexo, EstadoCivil,
    ///     Nacionalidad, Area, Cargo, etc.) viven en
    ///     Cepheus.Domain.Rrhh.Catalogos, con Id (long) como PK — igual
    ///     patrón que uses tú al crearlos. Si el namespace o el nombre de
    ///     alguna clase difiere, solo hay que corregir el using y el tipo
    ///     de la navegación, la forma de la entidad no cambia.
    ///   - moneda_id/banco_id del legacy se adaptaron para reutilizar los
    ///     catálogos Moneda/Banco YA EXISTENTES en Comunes (por su Code
    ///     natural, no por un Id bigint que esas tablas no tienen).
    ///
    /// IsActive/CreatedAt/CreatedBy/UpdatedAt/UpdatedBy/RowVersion no
    /// existen en el script legacy; se agregan por consistencia con el
    /// resto de entidades del sistema.
    /// </summary>
    public class TrabajadorLaboral : IAuditableEntity
    {
        public long Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public DateTime? FechaIngreso { get; set; } // legacy: fecha_ingreso
        public DateTime? FechaCese { get; set; } // legacy: fecha_cese
        public string? TipoTrabajadorCode { get; set; } // legacy: tipo_trabajador_id
        public TipoTrabajador? TipoTrabajador { get; set; }
        public string? CategoriaTrabajadorCode { get; set; } // legacy: categoria_trabajador_id
        public CategoriaTrabajador? CategoriaTrabajador { get; set; }
        public string? EstadoTrabajadorCode { get; set; } // legacy: estado_trabajador_id
        public EstadoTrabajador? EstadoTrabajador { get; set; }
        public string? AreaCode { get; set; } // legacy: area_id
        public Area? Area { get; set; }
        public string? OcupacionCode { get; set; } // legacy: ocupacion_id
        public Ocupacion? Ocupacion { get; set; }
        public string? SubOcupacionCode { get; set; } // legacy: sub_ocupacion_id
        public SubOcupacion? SubOcupacion { get; set; }
        public string? OficinaCode { get; set; } // legacy: oficina_id
        public Oficina? Oficina { get; set; }
        public string? PlantaCode { get; set; } // legacy: planta_id
        public Planta? Planta { get; set; }
        public string? CargoCode { get; set; } // legacy: cargo_id
        public Cargo? Cargo { get; set; }
        public string? NivelCode { get; set; } // legacy: nivel_id
        public NivelTrabajador? Nivel { get; set; }
        public string? RegimenLaboralCode { get; set; } // legacy: regimen_laboral_id
        public RegimenLaboral? RegimenLaboral { get; set; }
        public string? ProveedorCode { get; set; } // legacy: proveedor_id
        public Proveedor? Proveedor { get; set; }
        public bool? Permanente { get; set; } // legacy: permanente
        public bool? Pensionista { get; set; } // legacy: pensionista

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
