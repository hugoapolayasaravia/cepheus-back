using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Tipos de producto (Producto.TipoProductoCode) que admite una Hoja de Corte.
    /// Legacy/PB: viguetas = '16', bovedillas = '18' (pestañas del PB y retrieve de
    /// cada DataWindow).
    /// </summary>
    public static class HojaCorteProductoTipos
    {
        public const string Vigueta = "16";
        public const string Bovedilla = "18";

        public static bool IsValid(string tipoProductoCode)
            => tipoProductoCode == Vigueta || tipoProductoCode == Bovedilla;
    }

    /// <summary>
    /// Línea de Hoja de Corte (vigueta o bovedilla).
    ///
    /// Legacy: dbo.DHojaCorte (SQL Server). PK legacy y de esta entidad:
    /// (ano_hco, codigo_hco, codigo_tpr, codigo_prd, item_hco). El ítem es correlativo
    /// por tipo de producto dentro de la HC (el PB numera cada pestaña por separado), de
    /// modo que un mismo producto puede repetirse (ej. VIGUETA 105 en dos paños).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   ano_hco, codigo_hco -> Year, HojaCorteCode (FK compuesta -> HojaCorte, cascade)
    ///   codigo_tpr          -> ProductoTipoCode (FK compuesta -> Producto)
    ///   codigo_prd          -> ProductoCode     (parte de la FK compuesta -> Producto)
    ///   item_hco            -> ItemNumber
    ///   codigo_uni          -> UnitCode (FK -> Facturacion.UnidadMedidaVenta)
    ///   cantidad_hco        -> Quantity ("B" en la grilla)
    ///   pano_hco            -> Panel (paño; obligatorio en viguetas)
    ///   longitud_hco        -> Length ("A" en la grilla; obligatoria en viguetas)
    ///   cables_hco          -> CablesSide1 (cables lado 1)
    ///   cables2_hco         -> CablesSide2 (cables lado 2; solo si hay lado 1)
    ///   lot01_hco..lot03_hco-> Lot1..Lot3
    ///
    /// Columnas calculadas de la grilla (fórmulas de los DataWindow, no se persisten):
    ///   Lados        -> Sides       = (CablesSide1 > 0 ? 1 : 0) + (CablesSide2 > 0 ? 1 : 0)
    ///   Total A * B  -> TotalLength = Length * Quantity
    ///   Total Cables -> TotalCables = CablesSide1 + CablesSide2
    ///   Cortes B/12  -> Cuts        = Quantity / 12
    /// </summary>
    public class HojaCorteDetalle : IAuditableEntity
    {
        public string Year { get; set; } = default!;
        public string HojaCorteCode { get; set; } = default!;
        public HojaCorte HojaCorte { get; set; } = default!;

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public int ItemNumber { get; set; }

        public string UnitCode { get; set; } = default!;
        public UnidadMedidaVenta Unit { get; set; } = default!;

        public decimal Quantity { get; set; }

        public string? Panel { get; set; }
        public decimal? Length { get; set; }

        public decimal? CablesSide1 { get; set; }
        public decimal? CablesSide2 { get; set; }

        public string? Lot1 { get; set; }
        public string? Lot2 { get; set; }
        public string? Lot3 { get; set; }

        // Calculadas (no persistidas)
        public int Sides => ((CablesSide1 ?? 0) > 0 ? 1 : 0) + ((CablesSide2 ?? 0) > 0 ? 1 : 0);
        public decimal TotalLength => (Length ?? 0) * Quantity;
        public decimal TotalCables => (CablesSide1 ?? 0) + (CablesSide2 ?? 0);
        public decimal Cuts => Quantity / 12m;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
