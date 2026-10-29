using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common
{
    public class TipoAsientoResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public TipoCambioAplicable ExchangeRateType { get; set; }
        public ValorizacionAsiento Valuation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
