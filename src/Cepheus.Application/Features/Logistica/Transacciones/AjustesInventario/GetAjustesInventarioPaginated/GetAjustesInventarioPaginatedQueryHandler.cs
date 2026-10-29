using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjustesInventarioPaginated;

public sealed class GetAjustesInventarioPaginatedQueryHandler
    : IRequestHandler<GetAjustesInventarioPaginatedQuery, PagedResult<AjusteInventarioListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetAjustesInventarioPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<AjusteInventarioListadoResponse>> Handle(
        GetAjustesInventarioPaginatedQuery request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.Codigo_Pla!);

        IQueryable<AjusteInventario> query = _uow.Logistica.Transacciones.AjustesInventario
            .Query()
            .AsNoTracking()
            .Where(x => x.PlantaCode == planta);

        // SP: si Codigo_Aju <> 'T' se busca por código y se ignoran los demás filtros.
        if (!IsAll(request.Codigo_Aju))
        {
            var codigo = AjusteInventarioRules.Normalize(request.Codigo_Aju!);
            query = query.Where(x => x.Code == codigo);
        }
        else
        {
            var desde = request.Fecha_Ini!.Value.Date;
            var hasta = request.Fecha_Fin!.Value.Date.AddDays(1);
            query = query.Where(x => x.CreatedAt >= desde && x.CreatedAt < hasta);

            if (!IsAll(request.Codigo_Est) &&
                AjusteInventarioRules.TryParseEstado(request.Codigo_Est, out var estado))
            {
                query = query.Where(x => x.Estado == estado);
            }

            if (!IsAll(request.Codigo_Usu))
            {
                var usuario = request.Codigo_Usu!.Trim();
                query = query.Where(x => x.CreatedBy != null && x.CreatedBy.Contains(usuario));
            }

            if (!IsAll(request.Tipo_Aju) &&
                AjusteInventarioRules.TryParseTipo(request.Tipo_Aju, out var tipo))
            {
                query = query.Where(x => x.Detalles.Any(d => d.Tipo == tipo));
            }
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        // Orden por defecto del SP: Fecha_Pro DESC, Codigo_Aju DESC.
        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Code)
            : query.ApplySort(request.SortBy, sortDesc);

        var paged = await sorted.ToPagedResultAsync(pageNumber, pageSize, ct);

        return new PagedResult<AjusteInventarioListadoResponse>
        {
            Items = paged.Items.Select(AjusteInventarioMapper.MapListado).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    private static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
