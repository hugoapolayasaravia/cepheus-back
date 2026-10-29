using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.GetOrdenesServicioPaginated;

public sealed class GetOrdenesServicioPaginatedQueryHandler
    : IRequestHandler<GetOrdenesServicioPaginatedQuery, PagedResult<OrdenServicioListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetOrdenesServicioPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<OrdenServicioListadoResponse>> Handle(
        GetOrdenesServicioPaginatedQuery request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.Codigo_Pla!);

        IQueryable<OrdenServicio> query = _uow.Logistica.Transacciones.OrdenesServicio.Query()
            .AsNoTracking()
            .Include(x => x.Proveedor)
            .Include(x => x.ComprobantePago)
            .Where(x => x.PlantaCode == planta);

        // SP: si Codigo_NoI <> 'T' se busca por código; si es 'T' se filtra por rango de Fecha_Pro.
        if (!IsAll(request.Codigo_NoI))
        {
            var noi = OrdenServicioRules.Normalize(request.Codigo_NoI!);
            query = query.Where(x => x.Code == noi);
        }
        else
        {
            var desde = request.Fecha_Ini!.Value.Date;
            var hasta = request.Fecha_Fin!.Value.Date.AddDays(1);
            query = query.Where(x => x.CreatedAt >= desde && x.CreatedAt < hasta);
        }

        // Estos tres filtros se aplican siempre (como en el SP).
        if (!IsAll(request.Codigo_Prv))
        {
            var prv = OrdenServicioRules.Normalize(request.Codigo_Prv!);
            query = query.Where(x => x.ProveedorCode == prv);
        }

        if (!IsAll(request.Codigo_Est) && OrdenServicioRules.TryParseEstado(request.Codigo_Est, out var estado))
            query = query.Where(x => x.Estado == estado);

        if (!IsAll(request.Pago))
        {
            var pago = OrdenServicioRules.Normalize(request.Pago!);
            query = query.Where(x => x.FormaPagoCode == pago);
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        // Orden por defecto del SP: Fecha_Pro DESC, Codigo_NoI DESC.
        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Code)
            : query.ApplySort(request.SortBy, sortDesc);

        var paged = await sorted.ToPagedResultAsync(pageNumber, pageSize, ct);

        return new PagedResult<OrdenServicioListadoResponse>
        {
            Items = paged.Items.Select(OrdenServicioMapper.MapListado).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    private static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
