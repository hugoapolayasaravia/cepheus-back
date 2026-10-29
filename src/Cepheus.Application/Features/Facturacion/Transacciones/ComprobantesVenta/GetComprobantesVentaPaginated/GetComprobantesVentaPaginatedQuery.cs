using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.GetComprobantesVentaPaginated;

/// <summary>
/// Filtros ('T' o vacío = todos): tip (01/03), num, ord (OT), cli, obr, pla (planta), est (1 Pendiente, 2 Facturado,
/// 3 Cancelado, 4 Anulado), guia (número de guía facturada) y fecha o rango fIni/fFin. Sin filtros: los de hoy.
/// </summary>
public sealed class GetComprobantesVentaPaginatedQuery
    : PagedRequest, IRequest<PagedResult<ComprobanteVentaListadoResponse>>
{
    public string? tip { get; set; }
    public string? num { get; set; }
    public string? ord { get; set; }
    public string? cli { get; set; }
    public string? obr { get; set; }
    public string? pla { get; set; }
    public int? est { get; set; }
    public string? guia { get; set; }
    public DateTime? fecha { get; set; }
    public DateTime? fIni { get; set; }
    public DateTime? fFin { get; set; }
}

public sealed class GetComprobantesVentaPaginatedQueryHandler
    : IRequestHandler<GetComprobantesVentaPaginatedQuery, PagedResult<ComprobanteVentaListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetComprobantesVentaPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    private static string? Filter(string? v)
    {
        var t = v?.Trim();
        return string.IsNullOrEmpty(t) || t.Equals("T", StringComparison.OrdinalIgnoreCase) ? null : t;
    }

    public async Task<PagedResult<ComprobanteVentaListadoResponse>> Handle(
        GetComprobantesVentaPaginatedQuery r,
        CancellationToken ct)
    {
        IQueryable<ComprobanteVenta> query = _uow.Facturacion.Transacciones.ComprobantesVenta
            .Query().AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Obra)
            .Include(x => x.Detalles);

        var tip = Filter(r.tip);
        var num = Filter(r.num);
        var ord = Filter(r.ord);
        var cli = Filter(r.cli);
        var obr = Filter(r.obr);
        var pla = Filter(r.pla);
        var guia = Filter(r.guia);

        if (tip is not null)
        {
            var t = ComprobanteVentaRules.NormalizeType(tip);
            query = query.Where(x => x.TypeCode == t);
        }

        if (num is not null)
        {
            var n = ComprobanteVentaRules.NormalizeNumber(num);
            query = query.Where(x => x.Number == n);
        }
        else
        {
            if (ord is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeCode(ord);
                query = query.Where(x => x.OrdenTrabajoCode == v);
            }

            if (cli is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeFixed(cli, 5);
                query = query.Where(x => x.ClienteCode == v);
            }

            if (obr is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeFixed(obr, 3);
                query = query.Where(x => x.ObraCode == v);
            }

            if (pla is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeFixed(pla, 2);
                query = query.Where(x => x.PlantCode == v);
            }

            if (guia is not null)
            {
                var v = GuiaRemisionVentaRules.NormalizeNumber(guia);
                query = query.Where(x => x.Detalles.Any(d => d.GuiaNumber == v));
            }

            if (r.est.HasValue)
            {
                if (!Enum.IsDefined(typeof(EstadoComprobanteVenta), r.est.Value))
                {
                    throw new InvalidOperationException("Estado inválido.");
                }

                var s = (EstadoComprobanteVenta)r.est.Value;
                query = query.Where(x => x.Status == s);
            }

            var hasOther = ord is not null || cli is not null || obr is not null || pla is not null
                           || guia is not null || r.est.HasValue || tip is not null;

            if (r.fIni.HasValue && r.fFin.HasValue)
            {
                var desde = r.fIni.Value.Date;
                var hasta = r.fFin.Value.Date;
                query = query.Where(x => x.IssueDate >= desde && x.IssueDate <= hasta);
            }
            else if (r.fecha.HasValue || !hasOther)
            {
                var day = (r.fecha ?? DateTime.Today).Date;
                query = query.Where(x => x.IssueDate == day);
            }
        }

        var sorted = string.IsNullOrWhiteSpace(r.SortBy)
            ? query.OrderByDescending(x => x.IssueDate).ThenByDescending(x => x.Number)
            : query.ApplySort(r.SortBy, r.SortDesc ?? true);

        var paged = await sorted.ToPagedResultAsync(r.PageNumber ?? 1, r.PageSize ?? 10, ct);

        return new PagedResult<ComprobanteVentaListadoResponse>
        {
            Items = paged.Items.Select(c => new ComprobanteVentaListadoResponse
            {
                TypeCode = c.TypeCode,
                Number = c.Number,
                IssueDate = c.IssueDate,
                ClienteCode = c.ClienteCode,
                ClienteName = c.Cliente.Name,
                ObraCode = c.ObraCode,
                ObraName = c.Obra.Description,
                PlantCode = c.PlantCode,
                OrdenTrabajoCode = c.OrdenTrabajoCode,
                Origin = c.Origin,
                CurrencyCode = c.CurrencyCode,
                TotalAmount = c.TotalAmount,
                Status = c.Status,
                IsPrinted = c.IsPrinted,
                Lines = c.Detalles.Count
            }).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
