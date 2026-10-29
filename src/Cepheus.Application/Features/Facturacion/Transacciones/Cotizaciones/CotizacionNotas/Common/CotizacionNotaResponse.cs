using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.Common
{
    public class CotizacionNotaResponse
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public int Sequence { get; set; }

        public string Description { get; set; } = default!;
        public OpcionNotaCotizacion Option { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
