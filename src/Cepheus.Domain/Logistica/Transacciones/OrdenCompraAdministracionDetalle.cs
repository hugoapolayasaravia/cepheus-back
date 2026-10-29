// Cepheus.Domain/Logistica/Transacciones/OrdenCompraAdministracionDetalle.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Línea de una Orden de Compra de Administración. Legacy: dbo.MCompraD.
    /// No lleva artículo: es una glosa libre (no mueve stock).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_com    -> OrdenCompraAdministracionCode (parte de la PK)
    ///   Item_Art      -> ItemNumber (decimal(18,0) legacy -> int; parte de la PK)
    ///   Glosa         -> Glosa (varchar(500))
    ///   Codigo_Uni    -> UnidadMedidaCode (FK Logistica.UnidadMedida, char(2))
    ///   Cantidad_Art  -> Cantidad
    ///   Precio_Art    -> Precio
    ///   Descuento_Art -> Descuento (MONTO, no porcentaje; igual que OrdenCompraDetalle)
    ///   Total_Art     -> Total = Cantidad × Precio − Descuento
    ///   Codigo_Scc    -> SubCentroCostoCode (FK SubCentroCosto)
    ///   Cantidad_Ent  -> CantidadEntregada (informativo)
    /// </summary>
    public class OrdenCompraAdministracionDetalle : IAuditableEntity
    {
        public string OrdenCompraAdministracionCode { get; set; } = default!;
        public OrdenCompraAdministracion OrdenCompraAdministracion { get; set; } = default!;

        public int ItemNumber { get; set; }

        public string Glosa { get; set; } = default!;

        public string UnidadMedidaCode { get; set; } = default!;
        public UnidadMedida UnidadMedida { get; set; } = default!;

        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }

        public string SubCentroCostoCode { get; set; } = default!;
        public SubCentroCosto SubCentroCosto { get; set; } = default!;

        public decimal CantidadEntregada { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
