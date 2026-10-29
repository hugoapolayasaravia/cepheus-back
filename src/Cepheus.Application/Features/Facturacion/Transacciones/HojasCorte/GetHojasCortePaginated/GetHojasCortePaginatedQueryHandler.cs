using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.GetHojasCortePaginated;

public sealed class GetHojasCortePaginatedQueryHandler
    : IRequestHandler<GetHojasCortePaginatedQuery, PagedResult<HojaCorteListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetHojasCortePaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<HojaCorteListadoResponse>> Handle(
        GetHojasCortePaginatedQuery request,
        CancellationToken ct)
    {
        IQueryable<HojaCorte> query = _uow.Facturacion.Transacciones.HojasCorte
            .Query()
            .AsNoTracking()
            .Include(x => x.TipoHojaCorte)
            .Include(x => x.Tecnico).ThenInclude(t => t.Trabajador)
            .Include(x => x.Vendedor);

        var nro = request.p_nrohco?.Trim();

        if (!string.IsNullOrEmpty(nro) && HojaCorteRules.NormalizeCode(nro).Length == HojaCorteRules.CodeLength)
        {
            var year = HojaCorteRules.Normalize(request.p_anio ?? string.Empty);
            var code = HojaCorteRules.NormalizeCode(nro);
            query = query.Where(x => x.Year == year && x.Code == code);
        }
        else
        {
            if (request.p_fecini.HasValue)
            {
                var desde = request.p_fecini.Value.Date;
                query = query.Where(x => x.Date >= desde);
            }

            if (request.p_fecfin.HasValue)
            {
                var hasta = request.p_fecfin.Value.Date.AddDays(1);
                query = query.Where(x => x.Date < hasta);
            }

            var cli = request.p_codcli?.Trim();
            if (cli is { Length: 5 })
            {
                var v = cli.ToUpperInvariant();
                query = query.Where(x => x.ClienteCode == v);
            }

            var obr = request.p_codobr?.Trim();
            if (obr is { Length: 3 })
            {
                var v = obr.ToUpperInvariant();
                query = query.Where(x => x.ObraCode == v);
            }

            var ven = request.p_codven?.Trim();
            if (ven is { Length: 4 })
            {
                var v = ven.ToUpperInvariant();
                query = query.Where(x => x.VendedorCode == v);
            }

            var tec = request.p_codtec?.Trim();
            if (tec is { Length: 5 })
            {
                var v = tec.ToUpperInvariant();
                query = query.Where(x => x.TecnicoCode == v);
            }

            if (!string.IsNullOrWhiteSpace(request.p_codest) &&
                HojaCorteRules.TryParseEstado(request.p_codest, out var estado))
            {
                query = query.Where(x => x.Status == estado);
            }

            var tip = request.p_tiphco?.Trim();
            if (tip is { Length: 2 })
            {
                var v = tip.ToUpperInvariant();
                query = query.Where(x => x.TipoHojaCorteCode == v);
            }

            var nomCli = request.p_nomcli?.Trim();
            if (!string.IsNullOrEmpty(nomCli))
            {
                query = query.Where(x => x.ClientName.Contains(nomCli));
            }

            var nomObr = request.p_nomobr?.Trim();
            if (!string.IsNullOrEmpty(nomObr))
            {
                query = query.Where(x => x.WorkName.Contains(nomObr));
            }
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        // Orden del SP: fecha_hco DESC, codigo_hco DESC.
        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(x => x.Date).ThenByDescending(x => x.Code)
            : query.ApplySort(request.SortBy, sortDesc);

        var paged = await sorted.ToPagedResultAsync(pageNumber, pageSize, ct);

        return new PagedResult<HojaCorteListadoResponse>
        {
            Items = paged.Items.Select(HojaCorteMapper.MapListado).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
