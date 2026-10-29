using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Programación de Otros (todos los negocios menos Concreto) de una OT de Ventas; una OT puede tener varias (parciales).
    /// Legacy: programacion.dbo.Otros (+ OtrosDetalle).
    ///
    ///   Codigo_otr      -> Code (correlativo global de 8 dígitos, fn_NuevaProgramacion_OT_Otros)
    ///   Codigo_ped      -> OrdenTrabajoCode (FK -> OrdenTrabajoVenta)
    ///   Fecha_pro       -> ProgramDate;  Hora_pro -> ProgramTime ("HH:mm")
    ///   Transporte_Obs  -> TransportObservations
    ///   (planta)        -> PlantCode: el PB la exige al programar aunque Otros no tiene la columna; se guarda para que
    ///                      las guías sepan desde qué planta se despacha.
    ///   (estado)        -> Status (01 Pendiente | 04 Anulada | 11 Cerrada); el DDL entregado no trae la columna.
    ///
    /// NO se guardan: Direccion, Encargado, Telefono (obra y su responsable), Codigo_ven (la OT), fechor_ing, fechor_pro
    /// (auditoría), Distrito y numero_lad (marcados "NO DEBE IR").
    /// </summary>
    public class ProgramacionOtros : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajoVenta OrdenTrabajo { get; set; } = default!;

        public DateTime ProgramDate { get; set; }
        public string ProgramTime { get; set; } = default!;

        public string PlantCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string? TransportObservations { get; set; }

        public EstadoProgramacion Status { get; set; } = EstadoProgramacion.Pendiente;

        public ICollection<ProgramacionOtrosDetalle> Detalles { get; set; } = new List<ProgramacionOtrosDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
