using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common
{
    public class NotaCotizacionPlantillaResponse
    {
        public string NegocioCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public OpcionNotaCotizacion Option { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
