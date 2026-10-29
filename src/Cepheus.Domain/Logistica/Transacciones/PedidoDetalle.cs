// Cepheus.Domain/Logistica/Transacciones/PedidoDetalle.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Línea de detalle de un Pedido. Legacy: dbo.MPedidoDet.
    ///
    /// Cambio de PK frente al legacy (confirmado por el usuario): la PK
    /// legacy era (Codigo_Pla, Codigo_Ped, Codigo_Art, Descripcion_Art),
    /// lo cual mezclaba texto libre en la clave. Se reemplaza por
    /// (PlantaCode, PedidoCode, ItemNumber) usando el correlativo de línea
    /// (Item_Art) como parte de la PK — más robusto y evita colisiones.
    ///
    /// ArticuloCode SIN FK estricta a propósito: el DDL legacy no declara
    /// FK de Codigo_Art hacia MArticulos. Esto es intencional — cuando el
    /// usuario que hace el pedido no pertenece a Logística, digita la
    /// descripción libremente y puede no existir un Articulo real detrás.
    /// Por eso ArticuloCode es nullable y sin relación de navegación EF;
    /// DescripcionArticulo es lo único que siempre se garantiza.
    ///
    /// OrdenCompraCode (Codigo_Com) queda como campo inerte reservado para
    /// cuando exista el módulo de Compras (Orden de Compra) — mismo
    /// criterio que los campos contables inertes en Articulo.
    /// </summary>
    public class PedidoDetalle : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string PedidoCode { get; set; } = default!;
        public Pedido Pedido { get; set; } = default!;

        /// <summary>Correlativo de línea dentro del pedido (Item_Art).</summary>
        public int ItemNumber { get; set; }

        /// <summary>
        /// Código de Articulo, SIN FK estricta (ver comentario de clase).
        /// Puede no corresponder a un Articulo real cuando la línea es de
        /// descripción libre.
        /// </summary>
        public string? ArticuloCode { get; set; }

        public string DescripcionArticulo { get; set; } = default!;

        public string UnidadMedidaCode { get; set; } = "UN";
        public UnidadMedida UnidadMedida { get; set; } = default!;

        public decimal PrecioArticulo { get; set; }
        public decimal CantidadArticulo { get; set; }
        public decimal TotalArticulo { get; set; }

        public EstadoPedidoDetalle EstadoPedidoDetalle { get; set; } = EstadoPedidoDetalle.Pendiente;

        /// <summary>Pendiente de FK real: módulo de Compras (Orden de Compra) aún no existe.</summary>
        public string? OrdenCompraCode { get; set; }

        public string? ProveedorCode { get; set; }
        public Proveedor? Proveedor { get; set; }

        /// <summary>
        /// Suma informativa de CantidadTomada en CotizacionPedidoOrigen para esta
        /// línea. Se recalcula cada vez que cambia el origen de una Cotización.
        /// No bloquea nada — control manual, solo referencia para Logística.
        /// </summary>
        public decimal CantidadCotizada { get; set; }

        public ICollection<CotizacionPedidoOrigen> OrigenesCotizacion { get; set; } = new List<CotizacionPedidoOrigen>();


        /// <summary>
        /// Suma informativa de CantidadTomada en OrdenCompraPedidoOrigen para esta
        /// línea. Control manual, sin bloqueo — mismo criterio que CantidadCotizada.
        /// </summary>
        public decimal CantidadEnCompra { get; set; }

        public ICollection<OrdenCompraPedidoOrigen> OrigenesCompra { get; set; } = new List<OrdenCompraPedidoOrigen>();


        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}