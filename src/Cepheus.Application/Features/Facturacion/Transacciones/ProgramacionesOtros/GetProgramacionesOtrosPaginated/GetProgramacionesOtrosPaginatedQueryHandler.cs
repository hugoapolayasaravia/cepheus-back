using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.GetProgramacionesOtrosPaginated;

public sealed class GetProgramacionesOtrosPaginatedQueryHandler
    : IRequestHandler<GetProgramacionesOtrosPaginatedQuery, PagedResult<ProgramacionOtrosListadoResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetProgramacionesOtrosPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    private static string? Filter(string? v)
    {
        var t = v?.Trim();
        return string.IsNullOrEmpty(t) || t.Equals("T", StringComparison.OrdinalIgnoreCase) ? null : t;
    }

    public async Task<PagedResult<ProgramacionOtrosListadoResponse>> Handle(
        GetProgramacionesOtrosPaginatedQuery r,
        CancellationToken ct)
    {
        IQueryable<ProgramacionOtros> query = _uow.Facturacion.Transacciones.ProgramacionesOtros
            .Query().AsNoTracking()
            .Include(x => x.Planta)
            .Include(x => x.Detalles)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Cliente)
            .Include(x => x.OrdenTrabajo).ThenInclude(o => o.Obra);

        var pro = Filter(r.pro);
        var ord = Filter(r.ord);
        var cli = Filter(r.cli);
        var obr = Filter(r.obr);
        var ven = Filter(r.ven);
        var pla = Filter(r.pla);
        var est = Filter(r.est);
        var hasRange = r.fIni.HasValue && r.fFin.HasValue;

        if (pro is not null)
        {
            var code = ProgramacionOtrosRules.NormalizeCode(pro);
            query = query.Where(x => x.Code == code);
        }
        else
        {
            if (ord is not null)
            {
                var code = ProgramacionOrdenTrabajo.NormalizeCode(ord);
                query = query.Where(x => x.OrdenTrabajoCode == code);
            }

            if (cli is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeFixed(cli, 5);
                query = query.Where(x => x.OrdenTrabajo.ClienteCode == v);
            }

            if (obr is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeFixed(obr, 3);
                query = query.Where(x => x.OrdenTrabajo.ObraCode == v);
            }

            if (ven is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeFixed(ven, 4);
                query = query.Where(x => x.OrdenTrabajo.VendedorCode == v);
            }

            if (pla is not null)
            {
                var v = OrdenTrabajoVentaRules.NormalizeFixed(pla, 2);
                query = query.Where(x => x.PlantCode == v);
            }

            if (est is not null)
            {
                if (!int.TryParse(est, out var e) || !Enum.IsDefined(typeof(EstadoProgramacion), e))
                {
                    throw new InvalidOperationException("Estado inválido: use 01 (Pendiente), 04 (Anulada) o 11 (Cerrada).");
                }

                var status = (EstadoProgramacion)e;
                query = query.Where(x => x.Status == status);
            }

            if (hasRange)
            {
                var desde = r.fIni!.Value.Date;
                var hasta = r.fFin!.Value.Date;
                query = query.Where(x => x.ProgramDate >= desde && x.ProgramDate <= hasta);
            }
            else
            {
                var hasOther = ord is not null || cli is not null || obr is not null || ven is not null
                               || pla is not null || est is not null;

                if (r.fecha_pro.HasValue || !hasOther)
                {
                    var day = (r.fecha_pro ?? DateTime.Today).Date;
                    query = query.Where(x => x.ProgramDate == day);
                }
            }
        }

        var sorted = string.IsNullOrWhiteSpace(r.SortBy)
            ? query.OrderByDescending(x => x.ProgramDate).ThenByDescending(x => x.ProgramTime).ThenByDescending(x => x.Code)
            : query.ApplySort(r.SortBy, r.SortDesc ?? true);

        var paged = await sorted.ToPagedResultAsync(r.PageNumber ?? 1, r.PageSize ?? 10, ct);

        return new PagedResult<ProgramacionOtrosListadoResponse>
        {
            Items = paged.Items.Select(Map).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    private static ProgramacionOtrosListadoResponse Map(ProgramacionOtros p)
    {
        var ot = p.OrdenTrabajo;
        var quantity = p.Detalles.Sum(d => d.Quantity);
        var delivered = p.Detalles.Sum(d => d.DeliveredQuantity);

        return new ProgramacionOtrosListadoResponse
        {
            Code = p.Code,
            ProgramDate = p.ProgramDate,
            ProgramTime = p.ProgramTime,
            OrdenTrabajoCode = p.OrdenTrabajoCode,
            ClienteCode = ot.ClienteCode,
            ClienteName = ot.Cliente.Name,
            ObraCode = ot.ObraCode,
            ObraName = ot.Obra.Description,
            VendedorCode = ot.VendedorCode,
            PlantCode = p.PlantCode,
            PlantName = p.Planta.Name,
            Quantity = quantity,
            DeliveredQuantity = delivered,
            BalanceQuantity = quantity - delivered,
            Lines = p.Detalles.Count,
            TransportObservations = p.TransportObservations,
            Status = p.Status
        };
    }
}
