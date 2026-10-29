using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.GetHojaCorteByCode;

public sealed class GetHojaCorteByCodeQueryHandler : IRequestHandler<GetHojaCorteByCodeQuery, HojaCorteResponse>
{
    private readonly IUnitOfWork _uow;

    public GetHojaCorteByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<HojaCorteResponse> Handle(GetHojaCorteByCodeQuery request, CancellationToken ct)
        => HojaCorteReader.GetAsync(
            _uow,
            HojaCorteRules.Normalize(request.Year),
            HojaCorteRules.NormalizeCode(request.Code),
            ct);
}
