// Cepheus.Domain/Logistica/Transacciones/CotizacionPedidoOrigen.cs
using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Vínculo a nivel de línea entre una Cotización y los Pedidos que la
    /// originaron: una línea de CotizacionDetalle (un artículo) puede
    /// componerse de cantidades tomadas de varias líneas de PedidoDetalle
    /// (de Pedidos distintos), y una misma línea de Pedido puede repartirse
    /// entre varias Cotizaciones a lo largo del tiempo.
    ///
    /// El control de que no se "sobre-cotice" un Pedido es manual (no hay
    /// validación bloqueante) — PedidoDetalle.CantidadCotizada solo informa
    /// cuánto se ha tomado hasta ahora, para que Logística decida.
    ///
    /// Supuesto: todos los Pedidos de origen pertenecen a la misma Planta
    /// que la Cotización — a confirmar si no siempre aplica.
    /// </summary>
    public class CotizacionPedidoOrigen : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string CotizacionCode { get; set; } = default!;
        public string ArticuloCode { get; set; } = default!;
        public CotizacionDetalle CotizacionDetalle { get; set; } = default!;

        public string PedidoCode { get; set; } = default!;
        public int PedidoItemNumber { get; set; }
        public PedidoDetalle PedidoDetalle { get; set; } = default!;

        public decimal CantidadTomada { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}