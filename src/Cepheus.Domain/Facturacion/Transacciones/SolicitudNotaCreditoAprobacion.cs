using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Gestión de una etapa de la solicitud (una fila por etapa 01..05). Reemplaza
    /// los grupos de columnas repetidas Login_apr*/Fecha_apr*/Observa_*/IND_ESTADO_*
    /// de dbo.RCreVentasSol.
    /// </summary>
    public class SolicitudNotaCreditoAprobacion
    {
        public string Code { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public SolicitudNotaCredito Solicitud { get; set; } = default!;

        public EtapaSolicitudNotaCredito Stage { get; set; }
        public EstadoAprobacionSolicitud Status { get; set; } = EstadoAprobacionSolicitud.Pendiente;

        public int? ActedByUserId { get; set; }
        public string? ActedBy { get; set; }
        public DateTime? ActedAt { get; set; }
        public string? Observation { get; set; }
    }
}
