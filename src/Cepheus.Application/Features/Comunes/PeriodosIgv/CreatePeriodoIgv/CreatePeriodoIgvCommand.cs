using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.CreatePeriodoIgv;

public record CreatePeriodoIgvCommand(
    DateOnly FechaInicio,
    DateOnly? FechaFin,
    decimal PorcentajeIgv) : IRequest<PeriodoIgvResponse>;
