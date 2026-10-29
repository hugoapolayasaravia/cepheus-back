using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.UpdatePeriodoIgv;

public record UpdatePeriodoIgvCommand(
    int Id,
    DateOnly FechaInicio,
    DateOnly? FechaFin,
    decimal PorcentajeIgv,
    byte[] RowVersion) : IRequest<PeriodoIgvResponse>;
