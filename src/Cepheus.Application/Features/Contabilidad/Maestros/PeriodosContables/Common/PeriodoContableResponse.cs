using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common
{
    public class PeriodoContableResponse
    {
        public string Code { get; set; } = default!;
        public int Year { get; set; }
        public int Month { get; set; }
        public EstadoPeriodoContable Status { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? ClosedBy { get; set; }
        public DateTime? ReopenedAt { get; set; }
        public string? ReopenedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
