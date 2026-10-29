using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.CreatePeriodoIgv;

public class CreatePeriodoIgvCommandHandler : IRequestHandler<CreatePeriodoIgvCommand, PeriodoIgvResponse>
{
    private readonly IUnitOfWork _uow;

    public CreatePeriodoIgvCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PeriodoIgvResponse> Handle(CreatePeriodoIgvCommand request, CancellationToken ct)
    {
        var periodo = new PeriodoIgv
        {
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            PorcentajeIgv = request.PorcentajeIgv
        };

        await _uow.Comunes.PeriodosIgv.AddAsync(periodo, ct);
        await _uow.SaveChangesAsync(ct);

        return PeriodoIgvResponse.From(periodo);
    }
}
