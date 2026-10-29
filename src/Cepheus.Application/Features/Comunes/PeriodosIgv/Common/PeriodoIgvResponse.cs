using Cepheus.Domain.Comunes;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.Common;

public class PeriodoIgvResponse
{
    public int Id { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public decimal PorcentajeIgv { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    internal static PeriodoIgvResponse From(PeriodoIgv p) => new()
    {
        Id = p.Id,
        FechaInicio = p.FechaInicio,
        FechaFin = p.FechaFin,
        PorcentajeIgv = p.PorcentajeIgv,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
        CreatedBy = p.CreatedBy,
        UpdatedBy = p.UpdatedBy,
        RowVersion = p.RowVersion
    };
}
