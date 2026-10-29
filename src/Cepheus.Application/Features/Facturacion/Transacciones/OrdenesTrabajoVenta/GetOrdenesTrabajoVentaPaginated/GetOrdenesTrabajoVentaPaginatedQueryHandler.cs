using System.Linq.Expressions;
using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetOrdenesTrabajoVentaPaginated;

public sealed class GetOrdenesTrabajoVentaPaginatedQueryHandler
    : IRequestHandler<GetOrdenesTrabajoVentaPaginatedQuery, PagedResult<OrdenTrabajoVentaListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetOrdenesTrabajoVentaPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<OrdenTrabajoVentaListadoResponse>> Handle(
        GetOrdenesTrabajoVentaPaginatedQuery request,
        CancellationToken ct)
    {
        IQueryable<OrdenTrabajoVenta> query = _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query()
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Vendedor)
            .Include(x => x.Detalles);

        // Rol del usuario (etapa_mus_ot): Producción o Créditos.
        var vista = request.vista?.Trim().ToUpperInvariant();

        if (vista == "P")
        {
            query = query.Where(x =>
                (x.Stage == EtapaOrdenTrabajoVenta.AprobadaCredito || x.Stage == EtapaOrdenTrabajoVenta.GeneraProgramacion)
                && x.PassedToProduction);
        }
        else if (vista == "C")
        {
            query = query.Where(x => !x.IsModifiable);
        }

        var ord = request.ord?.Trim();

        if (!OrdenTrabajoVentaRules.IsAll(ord))
        {
            var code = OrdenTrabajoVentaRules.NormalizeCode(ord!);
            query = query.Where(x => x.Code == code);
        }
        else
        {
            var igualQueSp = BuildFilters(request, out var hasFilters);
            query = igualQueSp(query);

            var includePending = request.opt?.Trim() == "1";

            // Parte principal del SP: rango de fechas, o el día fecIni (+ tip) si no hay otros filtros.
            Expression<Func<OrdenTrabajoVenta, bool>>? main = null;

            if (request.fIni.HasValue && request.fFin.HasValue)
            {
                var rangeFrom = request.fIni.Value.Date;
                var rangeTo = request.fFin.Value.Date.AddDays(1);
                main = x => x.CreatedAt >= rangeFrom && x.CreatedAt < rangeTo;
            }
            else if (!hasFilters)
            {
                var day = request.fecIni?.Date ?? DateTime.Now.Date;
                var next = day.AddDays(1);
                var tip = request.tip?.Trim();

                if (!OrdenTrabajoVentaRules.IsAll(tip) && OrdenTrabajoVentaRules.TryParseTipo(tip, out var tipo))
                {
                    main = x => x.CreatedAt >= day && x.CreatedAt < next && x.Type == tipo;
                }
                else
                {
                    main = x => x.CreatedAt >= day && x.CreatedAt < next;
                }
            }

            // Con filtros y sin fechas el SP no limita por fecha (ya incluye lo pendiente).
            if (main is not null)
            {
                query = includePending
                    ? query.Where(Or(main, PendingExpression(request)))
                    : query.Where(main);
            }
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Code)
            : query.ApplySort(request.SortBy, sortDesc);

        var paged = await sorted.ToPagedResultAsync(pageNumber, pageSize, ct);

        var igv = await OrdenTrabajoVentaRules.GetIgvRateAsync(_uow, ct);

        // Condición de pago de la obra (datod): C contado / R crédito.
        var obraKeys = paged.Items.Select(i => new { i.ClienteCode, i.ObraCode }).Distinct().ToList();
        var creditByObra = new Dictionary<(string, string), bool>();

        foreach (var k in obraKeys)
        {
            var isCredit = await _uow.Facturacion.Maestros.Obras
                .Query().AsNoTracking()
                .Where(o => o.ClienteCode == k.ClienteCode && o.Code == k.ObraCode)
                .Select(o => o.FormaPagoVenta != null && o.FormaPagoVenta.IsCredit)
                .FirstOrDefaultAsync(ct);

            creditByObra[(k.ClienteCode, k.ObraCode)] = isCredit;
        }

        return new PagedResult<OrdenTrabajoVentaListadoResponse>
        {
            Items = paged.Items
                .Select(i => OrdenTrabajoVentaMapper.MapListado(i, igv, creditByObra[(i.ClienteCode, i.ObraCode)]))
                .ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    /// <summary>Filtros cli, obr, ven, est, tpr, prd, eta, ot (comunes a la parte principal y a las pendientes).</summary>
    private static Func<IQueryable<OrdenTrabajoVenta>, IQueryable<OrdenTrabajoVenta>> BuildFilters(
        GetOrdenesTrabajoVentaPaginatedQuery r,
        out bool hasFilters)
    {
        var filters = new List<Func<IQueryable<OrdenTrabajoVenta>, IQueryable<OrdenTrabajoVenta>>>();

        if (!OrdenTrabajoVentaRules.IsAll(r.cli))
        {
            var v = OrdenTrabajoVentaRules.NormalizeFixed(r.cli!, 5);
            filters.Add(q => q.Where(x => x.ClienteCode == v));
        }

        if (!OrdenTrabajoVentaRules.IsAll(r.obr))
        {
            var v = OrdenTrabajoVentaRules.NormalizeFixed(r.obr!, 3);
            filters.Add(q => q.Where(x => x.ObraCode == v));
        }

        if (!OrdenTrabajoVentaRules.IsAll(r.ven))
        {
            var v = OrdenTrabajoVentaRules.NormalizeFixed(r.ven!, 4);
            filters.Add(q => q.Where(x => x.VendedorCode == v));
        }

        if (!OrdenTrabajoVentaRules.IsAll(r.est) && OrdenTrabajoVentaRules.TryParseEstado(r.est, out var estado))
        {
            filters.Add(q => q.Where(x => x.Status == estado));
        }

        if (!OrdenTrabajoVentaRules.IsAll(r.tpr))
        {
            var v = OrdenTrabajoVentaRules.NormalizeFixed(r.tpr!, 2);
            filters.Add(q => q.Where(x => x.Detalles.Any(d => d.ProductoTipoCode == v)));
        }

        if (!OrdenTrabajoVentaRules.IsAll(r.prd))
        {
            var v = OrdenTrabajoVentaRules.NormalizeFixed(r.prd!, 4);
            filters.Add(q => q.Where(x => x.Detalles.Any(d => d.ProductoCode == v)));
        }

        if (!OrdenTrabajoVentaRules.IsAll(r.eta) && OrdenTrabajoVentaRules.TryParseEtapa(r.eta, out var etapa))
        {
            filters.Add(q => q.Where(x => x.Stage == etapa));
        }

        if (!OrdenTrabajoVentaRules.IsAll(r.ot))
        {
            var v = OrdenTrabajoVentaRules.NormalizeCode(r.ot!);
            filters.Add(q => q.Where(x => x.OriginCode == v));
        }

        hasFilters = filters.Count > 0;

        return q => filters.Aggregate(q, (acc, f) => f(acc));
    }

    /// <summary>
    /// Bloque "pendientes" del SP (opt = 1): etapa Pendiente, Activa, sin aprobador y fuera de la ventana de fechas.
    /// </summary>
    private static Expression<Func<OrdenTrabajoVenta, bool>> PendingExpression(GetOrdenesTrabajoVentaPaginatedQuery r)
    {
        var hasRange = r.fIni.HasValue && r.fFin.HasValue;
        var desde = hasRange ? r.fIni!.Value.Date.AddDays(-5) : DateTime.MinValue;
        var hasta = hasRange ? r.fFin!.Value.Date : DateTime.MaxValue;

        var hasAntes = r.fecIni.HasValue;
        var antes = hasAntes ? r.fecIni!.Value.Date.AddDays(-5) : DateTime.MaxValue;

        return x =>
            x.Stage == EtapaOrdenTrabajoVenta.Pendiente
            && x.Status == EstadoOrdenTrabajoVenta.Activo
            && (x.ApprovedBy == null || x.ApprovedBy == string.Empty)
            && (!hasRange || (x.CreatedAt >= desde && x.CreatedAt < hasta))
            && (!hasAntes || x.CreatedAt < antes);
    }

    private static Expression<Func<OrdenTrabajoVenta, bool>> Or(
        Expression<Func<OrdenTrabajoVenta, bool>> a,
        Expression<Func<OrdenTrabajoVenta, bool>> b)
    {
        var p = Expression.Parameter(typeof(OrdenTrabajoVenta), "x");
        var left = new ParameterReplacer(a.Parameters[0], p).Visit(a.Body)!;
        var right = new ParameterReplacer(b.Parameters[0], p).Visit(b.Body)!;

        return Expression.Lambda<Func<OrdenTrabajoVenta, bool>>(Expression.OrElse(left, right), p);
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ParameterReplacer(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _from ? _to : base.VisitParameter(node);
    }
}
