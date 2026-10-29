// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/Common/ImportacionResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common
{
    public class ImportacionResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public decimal PesoNeto { get; set; }
        public decimal PesoBruto { get; set; }
        public DateTime FechaPoliza { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal TotalFob { get; set; }
        public decimal TotalFlete { get; set; }
        public decimal TotalSeguro { get; set; }
        public decimal TotalAduana { get; set; }
        public decimal Advalorem { get; set; }
        public decimal Sobretasa { get; set; }
        public decimal Igv { get; set; }
        public decimal OtrosGastos { get; set; }
        public string Estado { get; set; } = default!;
        /// <summary>Solo se carga en GetByCode y en los endpoints de detalle; los listados paginados vienen sin detalle.</summary>
        public List<ImportacionDetalleResponse> Detalles { get; set; } = new();
        public List<ImportacionGastoResponse> Gastos { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    public class ImportacionGastoResponse
    {
        public string ProveedorCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public bool Afecto { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal NetoGasto { get; set; }
        public decimal NetoGastoInafecto { get; set; }
        public decimal Igv { get; set; }
        public decimal IgvExterior { get; set; }
        public decimal Total { get; set; }
        /// <summary>Prorrateo del gasto por artículo (se llena al recalcular el prorrateo).</summary>
        public List<ImportacionGastoArticuloResponse> Articulos { get; set; } = new();
        public byte[] RowVersion { get; set; } = default!;
    }

    public class ImportacionGastoArticuloResponse
    {
        public string ArticuloCode { get; set; } = default!;
        public decimal ValorGasto { get; set; }
        public decimal IgvGasto { get; set; }
        public decimal IgvExtGasto { get; set; }
    }

    public class ImportacionDetalleResponse
    {
        public string ProveedorCode { get; set; } = default!;
        public string ArticuloCode { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ValorFob { get; set; }
        public decimal Flete { get; set; }
        public decimal Seguro { get; set; }
        public decimal ValorAduana { get; set; }
        public decimal PorcentajeDet { get; set; }
        public decimal ValorDet { get; set; }
        public decimal AdvaloremDet { get; set; }
        public decimal SobretasaDet { get; set; }
        public decimal IgvDet { get; set; }
        public decimal OtrosGastosDet { get; set; }
        public string Estado { get; set; } = default!;
        public byte[] RowVersion { get; set; } = default!;
    }
}
