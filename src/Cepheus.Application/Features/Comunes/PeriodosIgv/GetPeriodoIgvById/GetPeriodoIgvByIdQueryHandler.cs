using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodoIgvById;

public class GetPeriodoIgvByIdQueryHandler : IRequestHandler<GetPeriodoIgvByIdQuery, PeriodoIgvResponse>
{
    private readonly IUnitOfWork _uow;

    public GetPeriodoIgvByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PeriodoIgvResponse> Handle(GetPeriodoIgvByIdQuery request, CancellationToken ct)
    {
        var periodo = await _uow.Comunes.PeriodosIgv.Query().AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Período de IGV {request.Id} no encontrado.");

        return PeriodoIgvResponse.From(periodo);
    }
}
