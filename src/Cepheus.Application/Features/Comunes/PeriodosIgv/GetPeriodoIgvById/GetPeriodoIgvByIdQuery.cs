using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodoIgvById;

public record GetPeriodoIgvByIdQuery(int Id) : IRequest<PeriodoIgvResponse>;
