using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetOrdenTrabajoVentaByCode;

public sealed class GetOrdenTrabajoVentaByCodeQueryHandler
    : IRequestHandler<GetOrdenTrabajoVentaByCodeQuery, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public GetOrdenTrabajoVentaByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<OrdenTrabajoVentaResponse> Handle(GetOrdenTrabajoVentaByCodeQuery request, CancellationToken ct)
        => OrdenTrabajoVentaReader.GetAsync(_uow, OrdenTrabajoVentaRules.NormalizeCode(request.Code), ct);
}
