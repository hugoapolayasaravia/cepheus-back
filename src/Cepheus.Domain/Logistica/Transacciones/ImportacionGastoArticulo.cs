// Cepheus.Domain/Logistica/Transacciones/ImportacionGastoArticulo.cs
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Parte de un gasto administrativo asignada a un artículo. Legacy: dbo.MImportacionGDet_Articulos.
    /// Datos calculados por ImportacionProrrateoCalculator (no se digitan), por eso no llevan auditoría.
    /// La PK legacy no incluye proveedor de la línea: (Planta, Importación, Proveedor del gasto, Documento, Artículo).
    /// </summary>
    public class ImportacionGastoArticulo
    {
        public string PlantaCode { get; set; } = default!;
        public string ImportacionCode { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public ImportacionGasto Gasto { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        /// <summary>Valor del gasto (neto afecto + inafecto) asignado al artículo, en la moneda del gasto.</summary>
        public decimal ValorGasto { get; set; }
        public decimal IgvGasto { get; set; }
        public decimal IgvExtGasto { get; set; }
    }
}
