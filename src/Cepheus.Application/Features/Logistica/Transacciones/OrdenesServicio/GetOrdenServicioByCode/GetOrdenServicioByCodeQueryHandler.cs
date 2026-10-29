using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.GetOrdenServicioByCode;

/// <summary>
/// Orden completa (cabecera + líneas). Reemplaza a Logi_sp_Listado_MOrdenServicio_I_Articulos y a los
/// datos que el PowerBuilder cargaba al seleccionar una orden.
/// </summary>
public sealed class GetOrdenServicioByCodeQueryHandler
    : IRequestHandler<GetOrdenServicioByCodeQuery, OrdenServicioResponse>
{
    private readonly IUnitOfWork _uow;

    public GetOrdenServicioByCodeQueryHandler(IUnitOfWork uow) => _uow = uow;

    public Task<OrdenServicioResponse> Handle(GetOrdenServicioByCodeQuery request, CancellationToken ct)
        => OrdenServicioReader.GetAsync(
            _uow,
            OrdenServicioRules.Normalize(request.PlantaCode),
            OrdenServicioRules.Normalize(request.Code),
            ct);
}
