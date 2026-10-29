// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GetImportacionPendientes/GetImportacionPendientesQuery.cs
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GetImportacionPendientes
{
    /// <summary>Equivale a Logi_sp_Listado_MImportaciones_Articulos_Pen (dw_con_importaciones_articulos_pen). ProveedorCode null o "T" = todos.</summary>
    public record GetImportacionPendientesQuery(string PlantaCode, string ImportacionCode, string? ProveedorCode)
        : IRequest<List<ImportacionPendienteResponse>>;

    public class ImportacionPendienteResponse
    {
        public string ProveedorCode { get; set; } = default!;
        public string ArticuloCode { get; set; } = default!;
        public string ArticuloName { get; set; } = default!;
        public string UnidadMedidaCode { get; set; } = default!;
        public decimal Cantidad { get; set; }
        /// <summary>round(round(ValorDet, 6) / Cantidad, 6), en soles.</summary>
        public decimal Precio { get; set; }
        /// <summary>ValorDet (costo en soles). Se calcula con POST .../prorrateo; hasta entonces puede estar en 0.</summary>
        public decimal Total { get; set; }
    }
}
