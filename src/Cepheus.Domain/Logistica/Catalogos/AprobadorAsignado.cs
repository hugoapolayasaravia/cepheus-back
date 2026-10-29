using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Asignación de un trabajador como aprobador para una combinación
    /// (Nivel, Tipo de Transacción, Unidad de Negocio, Moneda). Puede haber
    /// varios registros por combinación — uno o varios aprobadores por tema
    /// de disponibilidad o nivel de autoridad, confirmado con el usuario.
    /// Maestro del submódulo de Gestión de Aprobación de Transacciones,
    /// dentro de Logística.
    ///
    /// Legacy: dbo.UsuariosXNivel (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tra          -> TrabajadorCode (titular — el que aprueba en
    ///                          primera instancia)
    ///   Codigo_Niv/Tras/Une/Mon -> NivelCode/TipoTransaccionCode/
    ///                          UnidadNegocioCode/MonedaCode (FK compuesta
    ///                          -> RangoAprobacion: solo se puede asignar un
    ///                          aprobador donde ya existe un rango de monto
    ///                          definido)
    ///   cod_user_suplente   -> SuplenteTrabajadorCode: quien aprueba si el
    ///                          titular no está disponible (confirmado)
    ///   cod_user_superior   -> SuperiorTrabajadorCode: el jefe del área a
    ///                          cargo de titular y suplente — TAMBIÉN puede
    ///                          aprobar directamente, no es solo para
    ///                          escalar por timeout (confirmado)
    ///   Ind_Usuario_Sup     -> ELIMINADO, confirmado que no se usa.
    ///
    /// Los 3 (Titular, Suplente, Superior) son aprobadores válidos para esa
    /// combinación — cualquiera de los tres puede aprobar, no hay orden de
    /// prelación estricto entre ellos.
    ///
    /// TrabajadorCode/SuplenteTrabajadorCode/SuperiorTrabajadorCode se
    /// guardan como texto simple, SIN FK real todavía — apuntan a
    /// Trabajador, tabla en construcción aparte (mismo criterio que
    /// OrdenTrabajo.ResponsableCode en Mantenimiento).
    /// </summary>
    public class AprobadorAsignado : IAuditableEntity
    {
        public string NivelCode { get; set; } = default!;
        public string TipoTransaccionCode { get; set; } = default!;
        public string UnidadNegocioCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public RangoAprobacion RangoAprobacion { get; set; } = default!;

        /// <summary>Trabajador titular — parte de la clave: permite varios aprobadores por combinación.</summary>
        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public string? SuplenteTrabajadorCode { get; set; }
        public string? SuperiorTrabajadorCode { get; set; }

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
