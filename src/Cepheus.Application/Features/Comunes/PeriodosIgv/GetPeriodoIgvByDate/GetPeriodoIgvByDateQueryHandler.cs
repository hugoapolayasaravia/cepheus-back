using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodoIgvByDate;

public class GetPeriodoIgvByDateQueryHandler : IRequestHandler<GetPeriodoIgvByDateQuery, PeriodoIgvResponse>
{
    private readonly IUnitOfWork _uow;

    public GetPeriodoIgvByDateQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PeriodoIgvResponse> Handle(GetPeriodoIgvByDateQuery request, CancellationToken ct)
    {
        var dia = request.Date;

        var periodo = await _uow.Comunes.PeriodosIgv.Query().AsNoTracking()
            .Where(p => p.FechaInicio <= dia && (p.FechaFin == null || p.FechaFin >= dia))
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException(
                $"No hay un período de IGV vigente para la fecha {dia:yyyy-MM-dd}.");

        return PeriodoIgvResponse.From(periodo);
    }
}
