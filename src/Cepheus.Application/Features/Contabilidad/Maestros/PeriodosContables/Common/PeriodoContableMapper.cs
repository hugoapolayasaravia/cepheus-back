using Cepheus.Domain.Contabilidad.Maestros;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common
{
    public static class PeriodoContableMapper
    {
        public static PeriodoContableResponse Map(PeriodoContable e) => new()
        {
            Code = e.Code,
            Year = e.Year,
            Month = e.Month,
            Status = e.Status,
            ClosedAt = e.ClosedAt,
            ClosedBy = e.ClosedBy,
            ReopenedAt = e.ReopenedAt,
            ReopenedBy = e.ReopenedBy,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
