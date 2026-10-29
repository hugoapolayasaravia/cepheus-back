using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresosPaginated;

public sealed class GetNotaIngresosPaginatedQueryHandler
    : IRequestHandler<GetNotaIngresosPaginatedQuery, PagedResult<NotaIngresoListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetNotaIngresosPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<NotaIngresoListadoResponse>> Handle(
        GetNotaIngresosPaginatedQuery request, CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.Codigo_Pla!);

        IQueryable<NotaIngreso> query = _uow.Logistica.Transacciones.NotaIngresos.Query()
            .AsNoTracking()
            .Include(x => x.Proveedor)
            .Include(x => x.ComprobantePago)
            .Where(x => x.PlantaCode == planta);

        // SP: si Codigo_NoI <> 'T' se busca por código; si es 'T' se filtra por rango de Fecha_Pro.
        if (!IsAll(request.Codigo_NoI))
        {
            var noi = NotaIngresoRules.Normalize(request.Codigo_NoI!);
            query = query.Where(x => x.Code == noi);
        }
        else
        {
            var desde = request.Fecha_Ini!.Value.Date;
            var hasta = request.Fecha_Fin!.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaProceso >= desde && x.FechaProceso < hasta);
        }

        if (!IsAll(request.Codigo_Prv))
        {
            var prv = NotaIngresoRules.Normalize(request.Codigo_Prv!);
            query = query.Where(x => x.ProveedorCode == prv);
        }

        if (!IsAll(request.Codigo_Est) && NotaIngresoRules.TryParseEstado(request.Codigo_Est, out var estado))
            query = query.Where(x => x.Estado == estado);

        if (!IsAll(request.Pago))
        {
            var pago = NotaIngresoRules.Normalize(request.Pago!);
            query = query.Where(x => x.FormaPagoCode == pago);
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        // Orden por defecto del SP: Fecha_Pro DESC, Codigo_NoI DESC.
        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(x => x.FechaProceso).ThenByDescending(x => x.Code)
            : query.ApplySort(request.SortBy, sortDesc);

        var paged = await sorted.ToPagedResultAsync(pageNumber, pageSize, ct);

        return new PagedResult<NotaIngresoListadoResponse>
        {
            Items = paged.Items.Select(NotaIngresoMapper.MapListado).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    private static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
