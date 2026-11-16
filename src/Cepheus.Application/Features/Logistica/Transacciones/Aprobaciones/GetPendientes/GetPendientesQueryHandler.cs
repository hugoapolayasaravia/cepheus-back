using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetPendientes;

public sealed class GetPendientesQueryHandler
    : IRequestHandler<GetPendientesQuery, PagedResult<PendienteResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IAprobacionService _aprobacion;

    public GetPendientesQueryHandler(IUnitOfWork uow, IAprobacionService aprobacion)
    {
        _uow = uow;
        _aprobacion = aprobacion;
    }

    public async Task<PagedResult<PendienteResponse>> Handle(GetPendientesQuery request, CancellationToken ct)
    {
        var tipo = AprobacionTipos.Normalizar(request.Tipo);

        var filas = await BandejaAprobacion.LeerPendientesAsync(_uow, _aprobacion, tipo, request.Todos, ct);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();

            filas = filas.Where(f =>
                    f.Code.Contains(s, StringComparison.OrdinalIgnoreCase)
                    || f.PlantaCode.Contains(s, StringComparison.OrdinalIgnoreCase)
                    || (f.Contraparte?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (f.CreatedBy?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        var pageNumber = request.PageNumber is > 0 ? request.PageNumber.Value : 1;
        var pageSize = request.PageSize is > 0 ? request.PageSize.Value : 10;

        var items = filas
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new PendienteResponse
            {
                TipoTransaccion = f.TipoTransaccion,
                PlantaCode = f.PlantaCode,
                Code = f.Code,
                UnidadNegocioCode = f.UnidadNegocioCode,
                MonedaCode = f.MonedaCode,
                Monto = f.Monto,
                Fecha = f.Fecha,
                Contraparte = f.Contraparte,
                CreatedBy = f.CreatedBy
            })
            .ToList();

        return PagedResult<PendienteResponse>.Create(items, filas.Count, pageNumber, pageSize);
    }
}
