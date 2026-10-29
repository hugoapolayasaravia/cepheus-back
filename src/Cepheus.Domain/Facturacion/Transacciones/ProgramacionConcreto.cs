using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Programación de Concreto de una OT de Ventas (parcial: una OT puede tener varias). Legacy: programacion.dbo.Concreto
    /// (+ ConcretoAdicional, que NO se migra como tabla aparte: sus datos son los mismos del concreto y quedan en el detalle).
    ///
    ///   Codigo_pro            -> Code (correlativo global de 8 dígitos, fn_NuevaProgramacion_OT_Concreto)
    ///   Codigo_ped            -> OrdenTrabajoCode (FK -> OrdenTrabajoVenta)
    ///   Fecha_pro, Hora_pro   -> ProgramDate, ProgramTime (fecha y hora "HH:mm" programadas de despacho)
    ///   Codigo_pla            -> PlantCode (FK -> Comunes.Planta)
    ///   Codigo_Ele            -> ElementoCode (FK -> Facturacion.ElementoConcreto)
    ///   Transporte_Obs        -> TransportObservations
    ///   Servicio_lab          -> RequiresLabService (S/N)
    ///   codigo_est            -> Status (01 Pendiente | 04 Anulada | 11 Cerrada)
    ///
    /// NO se guardan (se leen de donde nacen): Cantidad, Cantidad_bom, Cantidad_ent, Codigo_prd, Clave, Bomba, Apoyo,
    /// Aditivo1-3, Producto171/172/20 (salen del detalle); Direccion, Encargado, Telefono (obra y su responsable);
    /// Codigo_ven (la OT); fechor_ing, fechor_pro (auditoría).
    /// </summary>
    public class ProgramacionConcreto : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajoVenta OrdenTrabajo { get; set; } = default!;

        public DateTime ProgramDate { get; set; }
        public string ProgramTime { get; set; } = default!;

        public string PlantCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string ElementoCode { get; set; } = default!;
        public ElementoConcreto Elemento { get; set; } = default!;

        public string? TransportObservations { get; set; }
        public bool RequiresLabService { get; set; }

        public EstadoProgramacion Status { get; set; } = EstadoProgramacion.Pendiente;

        public ICollection<ProgramacionConcretoDetalle> Detalles { get; set; } = new List<ProgramacionConcretoDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
