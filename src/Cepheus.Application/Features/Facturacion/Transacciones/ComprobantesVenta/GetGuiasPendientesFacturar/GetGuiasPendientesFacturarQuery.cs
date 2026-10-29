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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.GetGuiasPendientesFacturar;

/// <summary>
/// Guías de venta Pendientes de facturar de un cliente (cli obligatorio). Filtros: obr, ord, pla, tip (01/03: solo guías
/// de ese documento o sin documento asignado) y hasta (fecha de emisión máxima).
/// </summary>
public sealed class GetGuiasPendientesFacturarQuery : IRequest<List<GuiaPendienteFacturarResponse>>
{
    public string cli { get; set; } = default!;
    public string? obr { get; set; }
    public string? ord { get; set; }
    public string? pla { get; set; }
    public string? tip { get; set; }
    public DateTime? hasta { get; set; }
}

public sealed class GetGuiasPendientesFacturarQueryHandler
    : IRequestHandler<GetGuiasPendientesFacturarQuery, List<GuiaPendienteFacturarResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetGuiasPendientesFacturarQueryHandler(IUnitOfWork uow) => _uow = uow;

    private static string? Filter(string? v)
    {
        var t = v?.Trim();
        return string.IsNullOrEmpty(t) || t.Equals("T", StringComparison.OrdinalIgnoreCase) ? null : t;
    }

    public async Task<List<GuiaPendienteFacturarResponse>> Handle(GetGuiasPendientesFacturarQuery r, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.cli))
        {
            throw new InvalidOperationException("Indique el cliente (cli).");
        }

        var cli = OrdenTrabajoVentaRules.NormalizeFixed(r.cli, 5);

        var query = _uow.Facturacion.Transacciones.GuiasRemisionVenta
            .Query().AsNoTracking()
            .Include(g => g.Detalles)
            .Where(g => g.ClienteCode == cli
                        && g.Status == EstadoGuiaRemisionVenta.Pendiente
                        && g.Motive == MotivoGuiaRemisionVenta.Venta);

        if (Filter(r.obr) is { } obr)
        {
            var v = OrdenTrabajoVentaRules.NormalizeFixed(obr, 3);
            query = query.Where(g => g.ObraCode == v);
        }

        if (Filter(r.ord) is { } ord)
        {
            var v = OrdenTrabajoVentaRules.NormalizeCode(ord);
            query = query.Where(g => g.OrdenTrabajoCode == v);
        }

        if (Filter(r.pla) is { } pla)
        {
            var v = OrdenTrabajoVentaRules.NormalizeFixed(pla, 2);
            query = query.Where(g => g.PlantCode == v);
        }

        if (Filter(r.tip) is { } tip)
        {
            var t = ComprobanteVentaRules.NormalizeType(tip);
            query = query.Where(g => g.DocumentTypeCode == null
                                     || g.DocumentTypeCode == GuiaRemisionVentaRules.DocEnEspera
                                     || g.DocumentTypeCode == t);
        }

        if (r.hasta.HasValue)
        {
            var h = r.hasta.Value.Date;
            query = query.Where(g => g.IssueDate <= h);
        }

        var list = await query.OrderBy(g => g.IssueDate).ThenBy(g => g.Number).ToListAsync(ct);

        return list.Select(g => new GuiaPendienteFacturarResponse
        {
            Number = g.Number,
            IssueDate = g.IssueDate,
            PlantCode = g.PlantCode,
            ClienteCode = g.ClienteCode,
            ObraCode = g.ObraCode,
            OrdenTrabajoCode = g.OrdenTrabajoCode,
            DocumentTypeCode = g.DocumentTypeCode,
            Document1Number = g.Document1Number,
            CurrencyCode = g.CurrencyCode,
            Quantity = g.Detalles.Sum(d => d.Quantity),
            Amount = g.Detalles.Sum(d => Math.Round(d.Quantity * d.UnitPrice, 2, MidpointRounding.AwayFromZero))
        }).ToList();
    }
}
