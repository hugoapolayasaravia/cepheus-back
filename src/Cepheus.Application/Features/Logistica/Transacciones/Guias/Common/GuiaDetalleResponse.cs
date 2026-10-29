// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaDetalleResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    public class GuiaDetalleResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string GuiaCode { get; set; } = default!;

        public int ItemNumber { get; set; }

        public string ArticuloCode { get; set; } = default!;
        public string? ArticuloName { get; set; }
        public string? UnidadMedidaCode { get; set; }

        public decimal Cantidad { get; set; }
        public bool IsVerified { get; set; }

        public string Estado { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
