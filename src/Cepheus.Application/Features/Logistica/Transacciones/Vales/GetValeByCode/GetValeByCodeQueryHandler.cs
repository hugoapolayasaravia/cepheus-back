using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeByCode;

public sealed class GetValeByCodeQueryHandler
    : IRequestHandler<GetValeByCodeQuery, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public GetValeByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<ValeResponse> Handle(GetValeByCodeQuery request, CancellationToken ct)
        => ValeReader.GetAsync(
            _uow,
            ValeRules.Normalize(request.Codigo_Pla),
            ValeRules.Normalize(request.Codigo_Val),
            ct);
}
