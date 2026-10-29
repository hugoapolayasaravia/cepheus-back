using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresoByCode;

public sealed class GetNotaIngresoByCodeQueryHandler
    : IRequestHandler<GetNotaIngresoByCodeQuery, NotaIngresoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetNotaIngresoByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<NotaIngresoResponse> Handle(GetNotaIngresoByCodeQuery request, CancellationToken ct)
        => NotaIngresoReader.GetAsync(
            _uow,
            NotaIngresoRules.Normalize(request.Codigo_Pla),
            NotaIngresoRules.Normalize(request.Codigo_NoI),
            ct);
}
