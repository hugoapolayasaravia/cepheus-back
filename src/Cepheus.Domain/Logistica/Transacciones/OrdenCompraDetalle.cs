// Cepheus.Domain/Logistica/Transacciones/OrdenCompraDetalle.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Línea de artículo de una Orden de Compra. Legacy: dbo.MComprasDet.
    ///
    /// Cambio de PK frente al legacy (mismo criterio que CotizacionDetalle):
    /// la PK legacy era (Planta, Compra, Articulo, Pedido) — permitía varias
    /// filas del mismo artículo, una por cada Pedido de origen. Se reemplaza
    /// por (PlantaCode, OrdenCompraCode, ArticuloCode) — una sola fila por
    /// artículo, con el detalle de qué Pedidos aportaron cantidad en
    /// OrdenCompraPedidoOrigen.
    ///
    /// ArticuloCode SÍ tiene FK real (igual que en el legacy —
    /// FK_MComprasDet_MArticulos).
    ///
    /// DescuentoArticulo es MONTO, no porcentaje (confirmado): si el
    /// comprobante trae descuento en %, el usuario lo convierte manualmente.
    /// TotalArticulo = CantidadArticulo × PrecioArticulo − DescuentoArticulo.
    /// </summary>
    public class OrdenCompraDetalle : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenCompraCode { get; set; } = default!;
        public OrdenCompra OrdenCompra { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        /// <summary>Correlativo de línea solo para orden de despliegue (no es parte de la PK).</summary>
        public int ItemNumber { get; set; }

        public decimal CantidadArticulo { get; set; }
        public decimal PrecioArticulo { get; set; }
        public decimal DescuentoArticulo { get; set; }
        public decimal TotalArticulo { get; set; }

        /// <summary>Informativo, actualizado más adelante por el futuro módulo de Nota de Ingreso.</summary>
        public decimal CantidadEntregada { get; set; }

        public string SubCentroCostoCode { get; set; } = default!;
        public SubCentroCosto SubCentroCosto { get; set; } = default!;

        public ICollection<OrdenCompraPedidoOrigen> Origenes { get; set; } = new List<OrdenCompraPedidoOrigen>();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}