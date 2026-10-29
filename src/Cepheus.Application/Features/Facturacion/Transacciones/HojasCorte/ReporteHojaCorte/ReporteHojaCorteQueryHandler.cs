using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.ReporteHojaCorte;

public sealed class ReporteHojaCorteQueryHandler
    : IRequestHandler<ReporteHojaCorteQuery, IReadOnlyList<ReporteHojaCorteRow>>
{
    private readonly IUnitOfWork _uow;

    public ReporteHojaCorteQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ReporteHojaCorteRow>> Handle(ReporteHojaCorteQuery request, CancellationToken ct)
    {
        IQueryable<HojaCorte> query = _uow.Facturacion.Transacciones.HojasCorte.Query().AsNoTracking();

        var anio = request.p_anohco?.Trim();
        if (anio is { Length: 4 })
        {
            query = query.Where(h => h.Year == anio);
        }

        var nro = request.p_nrohco?.Trim();
        if (nro is { Length: 8 })
        {
            query = query.Where(h => h.Code == nro);
        }

        var cli = request.p_codcli?.Trim();
        if (cli is { Length: 5 })
        {
            var v = cli.ToUpperInvariant();
            query = query.Where(h => h.ClienteCode == v);
        }

        var obr = request.p_codobr?.Trim();
        if (obr is { Length: 3 })
        {
            var v = obr.ToUpperInvariant();
            query = query.Where(h => h.ObraCode == v);
        }

        var ven = request.p_codven?.Trim();
        if (ven is { Length: 4 })
        {
            var v = ven.ToUpperInvariant();
            query = query.Where(h => h.VendedorCode == v);
        }

        var tec = request.p_codtec?.Trim();
        if (tec is { Length: 5 })
        {
            var v = tec.ToUpperInvariant();
            query = query.Where(h => h.TecnicoCode == v);
        }

        if (!string.IsNullOrWhiteSpace(request.p_codest)
            && HojaCorteRules.TryParseEstado(request.p_codest, out var estado))
        {
            query = query.Where(h => h.Status == estado);
        }

        var tip = request.p_tiphco?.Trim();
        if (tip is { Length: 2 })
        {
            var v = tip.ToUpperInvariant();
            query = query.Where(h => h.TipoHojaCorteCode == v);
        }

        query = ApplyDateRange(query, request);

        var rows = await (
            from h in query
            from d in h.Detalles.DefaultIfEmpty()
            orderby h.Date descending, h.Year, h.Code descending, d.ItemNumber
            select new
            {
                h.Year,
                h.Code,
                h.Date,
                h.ClienteCode,
                h.ClientName,
                h.ObraCode,
                h.WorkName,
                h.Floor,
                h.Sector,
                h.TecnicoCode,
                TecnicoName = h.Tecnico.Trabajador.FirstNames,
                h.VendedorCode,
                VendedorAbbreviation = h.Vendedor.Abbreviation,
                h.WorkAddress,
                District = h.WorkAddressUbigeo.District,
                Province = h.WorkAddressUbigeo.Province,
                Department = h.WorkAddressUbigeo.Department,
                h.ViguetaDispatchDate,
                h.BovedillaDispatchDate,
                h.ClientViguetaDispatchDate,
                h.ClientBovedillaDispatchDate,
                h.StorageDate,
                h.CreatedAt,
                h.Status,
                h.TipoHojaCorteCode,
                TipoHojaCorteName = h.TipoHojaCorte.Name,
                h.Observations,
                h.Turn,
                h.ParentYear,
                h.ParentCode,
                ParentOrdenTrabajoCode = h.Parent == null ? null : h.Parent.OrdenTrabajoCode,
                h.OrdenTrabajoCode,
                OtStatus = h.OrdenTrabajo == null ? (EstadoOrdenTrabajoVenta?)null : h.OrdenTrabajo!.Status,
                OtApprovedBy = h.OrdenTrabajo == null ? null : h.OrdenTrabajo!.ApprovedBy,
                OtApprovedAt = h.OrdenTrabajo == null ? (DateTime?)null : h.OrdenTrabajo!.ApprovedAt,
                ApprovalPayment = h.OrdenTrabajo == null || h.OrdenTrabajo.Aprobacion == null
                    ? (TipoPagoAprobacion?)null
                    : h.OrdenTrabajo.Aprobacion.PaymentType,
                ClientGlobalLine = h.Cliente == null ? false : h.Cliente.HasGlobalCreditLine,
                ClientGlobalAmount = h.Cliente == null ? null : h.Cliente.GlobalCreditAmount,
                ClientPayIsCredit = h.Cliente == null ? (bool?)null : h.Cliente.FormaPagoVenta.IsCredit,
                ObraPayIsCredit = h.Obra == null || h.Obra.FormaPagoVenta == null
                    ? (bool?)null
                    : h.Obra.FormaPagoVenta.IsCredit,
                ProductoTipoCode = d == null ? null : d.ProductoTipoCode,
                ProductoTipoName = d == null ? null : d.Producto.TipoProducto.Name,
                Panel = d == null ? null : d.Panel,
                ProductoCode = d == null ? null : d.ProductoCode,
                ProductoName = d == null ? null : d.Producto.Name,
                Length = d == null ? null : d.Length,
                Quantity = d == null ? (decimal?)null : d.Quantity,
                CablesSide1 = d == null ? null : d.CablesSide1,
                CablesSide2 = d == null ? null : d.CablesSide2,
                Lot1 = d == null ? null : d.Lot1,
                Lot2 = d == null ? null : d.Lot2,
                Lot3 = d == null ? null : d.Lot3,
                HasDetail = d != null
            })
            .ToListAsync(ct);

        return rows.Select(r =>
        {
            int? sides = null;
            decimal? total = null;
            decimal? perSide = null;

            if (r.HasDetail)
            {
                sides = ((r.CablesSide1 ?? 0) > 1 ? 1 : 0) + ((r.CablesSide2 ?? 0) > 1 ? 1 : 0);
                total = Math.Round((r.Length ?? 1m) * (r.Quantity ?? 0m), 2);
                perSide = Math.Round((r.Quantity ?? 0m) * sides.Value, 2);
            }

            return new ReporteHojaCorteRow
            {
                Year = r.Year,
                Code = r.Code,
                Date = r.Date,
                ClienteCode = r.ClienteCode,
                ClientName = r.ClientName,
                ObraCode = r.ObraCode,
                WorkName = r.WorkName,
                Floor = r.Floor,
                Sector = r.Sector,
                TecnicoCode = r.TecnicoCode,
                TecnicoName = r.TecnicoName,
                VendedorCode = r.VendedorCode,
                VendedorAbbreviation = r.VendedorAbbreviation,
                Address = BuildAddress(r.WorkAddress, r.District, r.Province, r.Department),
                ViguetaDispatchDate = r.ViguetaDispatchDate,
                BovedillaDispatchDate = r.BovedillaDispatchDate,
                ClientViguetaDispatchDate = r.ClientViguetaDispatchDate,
                ClientBovedillaDispatchDate = r.ClientBovedillaDispatchDate,
                StorageDate = r.StorageDate,
                CreatedAt = r.CreatedAt,
                StatusName = r.Status.ToString(),
                TipoHojaCorteCode = r.TipoHojaCorteCode,
                TipoHojaCorteName = r.TipoHojaCorteName,
                Observations = r.Observations,
                Turn = r.Turn.ToString(),
                ParentReference = r.ParentYear is null || r.ParentCode is null
                    ? null
                    : $"{r.ParentYear}  {r.ParentCode}  {r.ParentOrdenTrabajoCode}".TrimEnd(),
                OrdenTrabajoCode = r.OrdenTrabajoCode,
                OrdenTrabajoApprovedBy = r.OtApprovedBy,
                OrdenTrabajoApprovedAt = r.OtApprovedAt,
                OrdenTrabajoStatusName = r.OtStatus is null
                    ? null
                    : r.OtStatus == EstadoOrdenTrabajoVenta.Cerrado ? "Atendido 100%" : r.OtStatus.Value.ToString(),
                PaymentCondition = GetPaymentCondition(r.ApprovalPayment, r.ClientGlobalLine, r.ClientGlobalAmount, r.ClientPayIsCredit, r.ObraPayIsCredit),
                ProductoTipoCode = r.ProductoTipoCode,
                ProductoTipoName = r.ProductoTipoName,
                Panel = r.Panel,
                ProductoCode = r.ProductoCode,
                ProductoName = r.ProductoName,
                Length = r.Length,
                Quantity = r.Quantity,
                CablesSide1 = r.CablesSide1,
                CablesSide2 = r.CablesSide2,
                Sides = sides,
                Total = total,
                QuantityPerSide = perSide,
                Lot1 = r.Lot1,
                Lot2 = r.Lot2,
                Lot3 = r.Lot3
            };
        }).ToList();
    }

    private static IQueryable<HojaCorte> ApplyDateRange(IQueryable<HojaCorte> query, ReporteHojaCorteQuery request)
    {
        var option = request.p_opcion?.Trim();
        var from = request.p_fecini?.Date;
        var to = request.p_fecfin?.Date.AddDays(1); // incluye todo el día final (SP: 23:59:59)

        switch (option)
        {
            case "1":
                if (from.HasValue) query = query.Where(h => h.Date >= from.Value);
                if (to.HasValue) query = query.Where(h => h.Date < to.Value);
                break;
            case "2":
                if (from.HasValue) query = query.Where(h => h.ClientViguetaDispatchDate >= from.Value);
                if (to.HasValue) query = query.Where(h => h.ClientViguetaDispatchDate < to.Value);
                break;
            case "3":
                if (from.HasValue) query = query.Where(h => h.ClientBovedillaDispatchDate >= from.Value);
                if (to.HasValue) query = query.Where(h => h.ClientBovedillaDispatchDate < to.Value);
                break;
        }

        return query;
    }

    /// <summary>Dirección - distrito - provincia - departamento (SP: LTRIM/RTRIM concatenados con " - ").</summary>
    private static string BuildAddress(string address, string? district, string? province, string? department)
        => string.Join(" - ", new[] { address, district, province, department }
            .Select(p => p?.Trim())
            .Where(p => !string.IsNullOrEmpty(p)));

    /// <summary>
    /// SP "datod": tipo de pago de la aprobación; si no hay aprobación, cliente con línea global y monto > 0 usa la forma de pago
    /// del cliente, en otro caso la de la obra. Forma de pago al contado = C, a crédito = R.
    /// </summary>
    private static string? GetPaymentCondition(
        TipoPagoAprobacion? approvalPayment,
        bool clientGlobalLine,
        decimal? clientGlobalAmount,
        bool? clientPayIsCredit,
        bool? obraPayIsCredit)
    {
        if (approvalPayment is not null)
        {
            return approvalPayment == TipoPagoAprobacion.Contado ? "C" : "R";
        }

        var isCredit = clientGlobalLine && clientGlobalAmount > 0 ? clientPayIsCredit : obraPayIsCredit;

        return isCredit is null ? null : isCredit.Value ? "R" : "C";
    }
}
