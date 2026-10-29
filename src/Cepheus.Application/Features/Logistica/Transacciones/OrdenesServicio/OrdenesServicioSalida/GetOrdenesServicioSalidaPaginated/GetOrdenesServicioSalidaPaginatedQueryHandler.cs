using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.GetOrdenesServicioSalidaPaginated;

public sealed class GetOrdenesServicioSalidaPaginatedQueryHandler
    : IRequestHandler<GetOrdenesServicioSalidaPaginatedQuery, PagedResult<OrdenServicioSalidaListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetOrdenesServicioSalidaPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<OrdenServicioSalidaListadoResponse>> Handle(
        GetOrdenesServicioSalidaPaginatedQuery request, CancellationToken ct)
    {
        var planta = request.Codigo_Pla!.Trim().ToUpperInvariant();

        IQueryable<OrdenServicioSalida> query = _uow.Logistica.Transacciones.OrdenesServicioSalida.Query()
            .AsNoTracking()
            .Include(x => x.Trabajador)
            .Where(x => x.PlantaCode == planta);

        if (!GetOrdenesServicioSalidaPaginatedQueryValidator.IsAll(request.Codigo_Val))
        {
            var val = request.Codigo_Val!.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code == val);
        }
        else
        {
            var desde = request.Fecha_Ini!.Value.Date;
            var hasta = request.Fecha_Fin!.Value.Date.AddDays(1);
            query = query.Where(x => x.CreatedAt >= desde && x.CreatedAt < hasta);
        }

        if (!GetOrdenesServicioSalidaPaginatedQueryValidator.IsAll(request.Codigo_Tra))
        {
            var tra = request.Codigo_Tra!.Trim().ToUpperInvariant();
            query = query.Where(x => x.TrabajadorCode == tra);
        }

        if (!GetOrdenesServicioSalidaPaginatedQueryValidator.IsAll(request.Codigo_Est) &&
            GetOrdenesServicioSalidaPaginatedQueryValidator.TryParseEstado(request.Codigo_Est, out var estado))
        {
            query = query.Where(x => x.Estado == estado);
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Code)
            : query.ApplySort(request.SortBy, sortDesc);

        var paged = await sorted.ToPagedResultAsync(pageNumber, pageSize, ct);

        return new PagedResult<OrdenServicioSalidaListadoResponse>
        {
            Items = paged.Items.Select(OrdenServicioSalidaMapper.MapListado).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
