using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Domain.Contabilidad.Maestros
{
    /// <summary>
    /// Período contable (un registro por año y mes). Control GLOBAL de la contabilidad del negocio
    /// (no es por planta).
    ///
    /// Reemplaza a la variable global gs_cierre del legacy (PowerBuilder), que guardaba el último
    /// año-mes cerrado y se comparaba con "anomes &lt;= gs_cierre". Aquí cada mes es un registro
    /// con su propio estado.
    ///
    /// Code = "yyyyMM" (PK natural, char(6)), por ejemplo "202610".
    ///
    /// Regla de uso: generar o anular un asiento exige que el período exista y esté Abierto.
    /// </summary>
    public class PeriodoContable : IAuditableEntity
    {
        /// <summary>Año y mes en formato yyyyMM (PK natural, 6 caracteres).</summary>
        public string Code { get; set; } = default!;

        public int Year { get; set; }

        public int Month { get; set; }

        public EstadoPeriodoContable Status { get; set; } = EstadoPeriodoContable.Abierto;

        public DateTime? ClosedAt { get; set; }
        public string? ClosedBy { get; set; }

        public DateTime? ReopenedAt { get; set; }
        public string? ReopenedBy { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
