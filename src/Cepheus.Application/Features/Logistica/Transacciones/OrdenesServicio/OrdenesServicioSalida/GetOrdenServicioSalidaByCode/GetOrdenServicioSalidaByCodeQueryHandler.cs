using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.GetOrdenServicioSalidaByCode;

public sealed class GetOrdenServicioSalidaByCodeQueryHandler
    : IRequestHandler<GetOrdenServicioSalidaByCodeQuery, OrdenServicioSalidaResponse>
{
    private readonly IUnitOfWork _uow;

    public GetOrdenServicioSalidaByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<OrdenServicioSalidaResponse> Handle(GetOrdenServicioSalidaByCodeQuery request, CancellationToken ct)
        => OrdenServicioSalidaReader.GetAsync(
            _uow,
            request.PlantaCode.Trim().ToUpperInvariant(),
            request.Code.Trim().ToUpperInvariant(),
            ct);
}
