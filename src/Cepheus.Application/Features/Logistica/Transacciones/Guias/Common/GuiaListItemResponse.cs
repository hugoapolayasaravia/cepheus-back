// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaListItemResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    /// <summary>
    /// Fila del listado paginado (equivale a las columnas de
    /// Logi_sp_Listado_MGuias: planta, guía, fecha, usuario, proveedor,
    /// motivo y estado). No incluye el detalle.
    /// </summary>
    public class GuiaListItemResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; }

        public string Code { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
        public string Hora { get; set; } = default!;

        /// <summary>Usuario que emitió la guía (Usuario en el legacy).</summary>
        public string? CreatedBy { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }

        public string MotivoCode { get; set; } = default!;
        public string? MotivoName { get; set; }

        public string Estado { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
