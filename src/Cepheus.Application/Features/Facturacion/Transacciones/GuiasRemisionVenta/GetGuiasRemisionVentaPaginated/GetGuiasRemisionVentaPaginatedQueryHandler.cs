using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.GetGuiasRemisionVentaPaginated;

public sealed class GetGuiasRemisionVentaPaginatedQueryHandler
    : IRequestHandler<GetGuiasRemisionVentaPaginatedQuery, PagedResult<GuiaRemisionVentaListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetGuiasRemisionVentaPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    private static string? Filter(string? v)
    {
        var t = v?.Trim();
        return string.IsNullOrEmpty(t) || t.Equals("T", StringComparison.OrdinalIgnoreCase) ? null : t;
    }

    public async Task<PagedResult<GuiaRemisionVentaListadoResponse>> Handle(
        GetGuiasRemisionVentaPaginatedQuery r,
        CancellationToken ct)
    {
        IQueryable<GuiaRemisionVenta> query = _uow.Facturacion.Transacciones.GuiasRemisionVenta
            .Query().AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Obra)
            .Include(x => x.Detalles);

        var gui = Filter(r.gui);
        var ord = Filter(r.ord);
        var pro = Filter(r.pro);
        var cli = Filter(r.cli);
        var obr = Filter(r.obr);
        var pla = Filter(r.pla);
        var doc = Filter(r.doc);

        if (gui is not null)
        {
            var number = GuiaRemisionVentaRules.NormalizeNumber(gui);
            query = query.Where(x => x.Number == number);
        }
        else
        {
            if (ord is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeCode(ord);
                query = query.Where(x => x.OrdenTrabajoCode == v);
            }

            if (pro is not null)
            {
                var v = GuiaRemisionVentaProgramaciones.NormalizeCode(pro);
                query = query.Where(x => x.ProgramacionCode == v);
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

            if (doc is not null)
            {
                var v = doc.ToUpperInvariant();
                query = query.Where(x => x.Document1Number == v || x.Document2Number == v);
            }

            if (r.est.HasValue)
            {
                if (!Enum.IsDefined(typeof(EstadoGuiaRemisionVenta), r.est.Value))
                {
                    throw new InvalidOperationException("Estado inválido.");
                }

                var s = (EstadoGuiaRemisionVenta)r.est.Value;
                query = query.Where(x => x.Status == s);
            }

            if (r.mot.HasValue)
            {
                var m = GuiaRemisionVentaRules.ParseMotive(r.mot.Value);
                query = query.Where(x => x.Motive == m);
            }

            var hasOther = ord is not null || pro is not null || cli is not null || obr is not null || pla is not null
                           || doc is not null || r.est.HasValue || r.mot.HasValue;

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

        return new PagedResult<GuiaRemisionVentaListadoResponse>
        {
            Items = paged.Items.Select(g => new GuiaRemisionVentaListadoResponse
            {
                Number = g.Number,
                IssueDate = g.IssueDate,
                Motive = g.Motive,
                Status = g.Status,
                ClienteCode = g.ClienteCode,
                ClienteName = g.Cliente.Name,
                ObraCode = g.ObraCode,
                ObraName = g.Obra.Description,
                PlantCode = g.PlantCode,
                OrdenTrabajoCode = g.OrdenTrabajoCode,
                ProgramacionType = g.ProgramacionType,
                ProgramacionCode = g.ProgramacionCode,
                DocumentTypeCode = g.DocumentTypeCode,
                Document1Number = g.Document1Number,
                Quantity = g.Detalles.Sum(d => d.Quantity),
                IsPrinted = g.IsPrinted
            }).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }
}
