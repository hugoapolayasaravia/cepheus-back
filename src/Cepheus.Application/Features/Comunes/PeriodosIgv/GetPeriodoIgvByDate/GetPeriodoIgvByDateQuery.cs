using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodoIgvByDate;

/// <summary>Período de IGV que rige en una fecha puntual (el fg_igv(fecha) del legacy).</summary>
public record GetPeriodoIgvByDateQuery(DateOnly Date) : IRequest<PeriodoIgvResponse>;
