using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjusteInventarioByCode;

public sealed class GetAjusteInventarioByCodeQueryHandler
    : IRequestHandler<GetAjusteInventarioByCodeQuery, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;

    public GetAjusteInventarioByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<AjusteInventarioResponse> Handle(
        GetAjusteInventarioByCodeQuery request,
        CancellationToken ct)
        => AjusteInventarioReader.GetAsync(
            _uow,
            AjusteInventarioRules.Normalize(request.Codigo_Pla),
            AjusteInventarioRules.Normalize(request.Codigo_Aju),
            ct);
}
