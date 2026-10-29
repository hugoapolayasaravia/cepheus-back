using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Control de cierre de operaciones del módulo de Logística, por planta y
    /// período (año-mes). La existencia de un registro para una combinación
    /// (Planta, Período) indica que ese período quedó cerrado.
    ///
    /// Legacy: dbo.TControl (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   CODIGO_PLA -> PlantaCode (FK -> Comunes.Planta.Code; mismo criterio
    ///                 confirmado en ArticuloProveedor/ArticuloStock/
    ///                 CentroCosto/SubCentroCosto: Codigo_Pla = Planta)
    ///   DCIERRE    -> PeriodCode (PK compuesta junto con PlantaCode, char(6),
    ///                 formato AAAAMM — confirmado por el comentario extendido
    ///                 del script legacy: "Código de año y mes de Cierre")
    ///   DFECHA     -> ClosureDate
    ///   Monto_Dif  -> DifferenceAmount (diferencia encontrada en la
    ///                 conciliación del cierre)
    ///
    /// El legacy no tiene columna de auditoría; se agrega por consistencia.
    /// No hay un campo de estado — no existe en el script un concepto de
    /// "reabrir" un cierre, así que no se expone Toggle/Delete: solo
    /// Create (registrar el cierre) y Update (corregir el monto de
    /// diferencia si aparece un ajuste posterior).
    /// </summary>
    public class ControlCierre : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>Período del cierre, formato AAAAMM (ej. "202401").</summary>
        public string PeriodCode { get; set; } = default!;

        public DateTime ClosureDate { get; set; }

        public decimal DifferenceAmount { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
