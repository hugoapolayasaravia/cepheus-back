using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Catalogos;
using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Domain.Mantenimiento.Transacciones
{
    /// <summary>
    /// Horas de parada no programada de una unidad (subcentro de costo) en
    /// un día. Registro diario: una fila por unidad y fecha.
    ///
    /// Legacy: dbo.MUnidad_Parada (SQL Server). Se captura desde la ventana
    /// w_log_equipo_paradas ("Horas de Paradas no programadas").
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_scc  -> SubCentroCostoCode (FK -> Logistica.Maestros.
    ///                  SubCentroCosto.Code, char(6))
    ///   Fecha_Hor   -> Fecha (parte fecha; la ventana siempre graba 00:00,
    ///                  por eso smalldatetime -> date, igual que el resto
    ///                  del repo)
    ///   Horas_Pad   -> Horas (decimal(18,2), default 0)
    ///   Estado_Equ  -> Estado (enum EstadoEquipo)
    ///   Planta_Afe  -> PlantaAfectadaCode (FK -> Comunes.Planta.Code)
    ///   Codigo_Fal  -> FallaCode (FK -> Mantenimiento.Catalogos.Falla.Code,
    ///                  nullable tal cual el legacy)
    ///
    /// PK compuesta (SubCentroCostoCode, Fecha), igual que PK_MUnidad_Parada.
    /// Reglas del legacy: el SP Logi_sp_AgregaMUnidad_Parada no registra
    /// nada si Horas = 0 y la ventana valida 0 a 12 horas por día.
    /// </summary>
    public class UnidadParada : IAuditableEntity
    {
        public string SubCentroCostoCode { get; set; } = default!;
        public SubCentroCosto SubCentroCosto { get; set; } = default!;

        public DateTime Fecha { get; set; }

        public decimal Horas { get; set; }

        public EstadoEquipo Estado { get; set; }

        public string PlantaAfectadaCode { get; set; } = default!;
        public Planta PlantaAfectada { get; set; } = default!;

        public string? FallaCode { get; set; }
        public Falla? Falla { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
