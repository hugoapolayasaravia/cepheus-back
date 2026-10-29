using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValesPaginated;

public sealed class GetValesPaginatedQueryHandler
    : IRequestHandler<GetValesPaginatedQuery, PagedResult<ValeListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetValesPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<ValeListadoResponse>> Handle(
        GetValesPaginatedQuery request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.Codigo_Pla!);

        IQueryable<Vale> query = ValeReader
            .WithListadoIncludes(_uow.Logistica.Transacciones.Vales.Query())
            .Where(x => x.PlantaCode == planta);

        // SP: si Codigo_Val <> 'T' se busca por código y se ignoran los demás filtros.
        if (!IsAll(request.Codigo_Val))
        {
            var codigo = ValeRules.Normalize(request.Codigo_Val!);
            query = query.Where(x => x.Code == codigo);
        }
        else
        {
            if (request.Fecha_Ini.HasValue)
            {
                var desde = request.Fecha_Ini.Value.Date;
                var hasta = request.Fecha_Fin!.Value.Date.AddDays(1);
                query = query.Where(x => x.CreatedAt >= desde && x.CreatedAt < hasta);
            }

            if (!IsAll(request.Codigo_Est) &&
                ValeRules.TryParseEstado(request.Codigo_Est, out var estado))
            {
                query = query.Where(x => x.Estado == estado);
            }

            if (!IsAll(request.Codigo_Usu))
            {
                var usuario = request.Codigo_Usu!.Trim();
                query = query.Where(x => x.CreatedBy != null && x.CreatedBy.Contains(usuario));
            }

            if (!IsAll(request.Responsable))
            {
                var responsable = ValeRules.Normalize(request.Responsable!);
                query = query.Where(x => x.TrabajadorCode == responsable);
            }
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        // Orden por defecto del SP: Fecha_Pro DESC, Codigo_Val DESC.
        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Code)
            : query.ApplySort(request.SortBy, sortDesc);

        var paged = await sorted.ToPagedResultAsync(pageNumber, pageSize, ct);

        return new PagedResult<ValeListadoResponse>
        {
            Items = paged.Items.Select(ValeMapper.MapListado).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    private static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
