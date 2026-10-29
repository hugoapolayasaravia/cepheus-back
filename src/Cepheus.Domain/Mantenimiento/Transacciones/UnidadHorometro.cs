using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Enum;
using Cepheus.Domain.Mantenimiento.Maestros;

namespace Cepheus.Domain.Mantenimiento.Transacciones
{
    /// <summary>
    /// Lectura diaria del horómetro de una unidad (subcentro de costo).
    /// Registro diario: una fila por unidad y fecha.
    ///
    /// Legacy: dbo.MUnidad_Horometro (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_scc  -> SubCentroCostoCode (FK -> Logistica.Maestros.
    ///                  SubCentroCosto.Code, char(6))
    ///   Fecha_Hor   -> Fecha (smalldatetime -> date)
    ///   Horometro   -> Horometro (decimal(18,2), default 0)
    ///   Estado_Equ  -> Estado (enum EstadoEquipo)
    ///   Planta_Afe  -> PlantaAfectadaCode (FK -> Comunes.Planta.Code)
    ///   Codigo_Ori  -> EquipoCode (FK -> Mantenimiento.Maestros.Equipo.Code;
    ///                  Equipo es la MOrigenes del legacy, nullable)
    ///
    /// PK compuesta (SubCentroCostoCode, Fecha), igual que
    /// PK_MUnidad_Horometro.
    /// </summary>
    public class UnidadHorometro : IAuditableEntity
    {
        public string SubCentroCostoCode { get; set; } = default!;
        public SubCentroCosto SubCentroCosto { get; set; } = default!;

        public DateTime Fecha { get; set; }

        public decimal Horometro { get; set; }

        public EstadoEquipo Estado { get; set; }

        public string PlantaAfectadaCode { get; set; } = default!;
        public Planta PlantaAfectada { get; set; } = default!;

        public string? EquipoCode { get; set; }
        public Equipo? Equipo { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
