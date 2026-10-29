using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.SeguimientoOrdenCompra.GetReporteSeguimientoOrdenCompra;

/// <summary>
/// Solo lectura, sin tablas temporales ni cursor: reemplaza Logi_sp_Rpt_Seguimiento_OC_GI_NI.
/// </summary>
public sealed class GetReporteSeguimientoOrdenCompraQueryHandler
    : IRequestHandler<GetReporteSeguimientoOrdenCompraQuery, ReporteSeguimientoOrdenCompraResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteSeguimientoOrdenCompraQueryHandler(IUnitOfWork uow) => _uow = uow;

    private sealed record Base(
        string ProveedorCode, string? ProveedorNombre, string OrdenCode, DateTime Fecha, string Moneda,
        string ArticuloCode, string ArticuloNombre, string Unidad, decimal Cantidad, decimal Entregada, decimal Precio,
        EstadoOrdenCompra Estado, string? PedidoCode);

    public async Task<ReporteSeguimientoOrdenCompraResponse> Handle(
        GetReporteSeguimientoOrdenCompraQuery request, CancellationToken ct)
    {
        var planta = ReporteRules.Normalizar(request.PlantaCode);
        var oc = ReporteRules.Filtro(request.OrdenCompraCode);
        var proveedor = ReporteRules.Filtro(request.ProveedorCode);
        var articulo = ReporteRules.Filtro(request.ArticuloCode);
        var fam = ReporteRules.Filtro(request.FamiliaCode);
        var sfa = ReporteRules.Filtro(request.SubFamiliaCode);

        var q = _uow.Logistica.Transacciones.OrdenCompraDetalles.Query().AsNoTracking()
            .Where(d => d.PlantaCode == planta);

        if (oc is not null) q = q.Where(d => d.OrdenCompraCode == oc);
        if (proveedor is not null) q = q.Where(d => d.OrdenCompra.ProveedorCode == proveedor);
        if (articulo is not null) q = q.Where(d => d.ArticuloCode == articulo);
        if (fam is not null) q = q.Where(d => d.Articulo.SubFamilia.FamiliaCode == fam);
        if (sfa is not null) q = q.Where(d => d.Articulo.SubFamiliaCode == sfa);

        if (request.FechaDesde.HasValue && request.FechaHasta.HasValue)
        {
            var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde.Value, request.FechaHasta.Value);
            q = q.Where(d => d.OrdenCompra.CreatedAt >= desde && d.OrdenCompra.CreatedAt < hastaExcl);
        }

        var lineas = await q.Select(d => new
        {
            d.OrdenCompra.ProveedorCode,
            ProveedorNombre = d.OrdenCompra.Proveedor.LegalName,
            OrdenCode = d.OrdenCompraCode,
            Fecha = d.OrdenCompra.CreatedAt,
            Moneda = d.OrdenCompra.MonedaCode,
            d.ArticuloCode,
            ArticuloNombre = d.Articulo.Name,
            Unidad = d.Articulo.UnidadMedidaCode,
            Cantidad = d.CantidadArticulo,
            Entregada = d.CantidadEntregada,
            Precio = d.PrecioArticulo,
            Estado = d.OrdenCompra.Estado,
            Pedidos = d.Origenes.Select(o => o.PedidoCode).ToList()
        }).ToListAsync(ct);

        // Un artículo puede venir de varios pedidos: una fila base por pedido de origen (o una sola sin pedido).
        var bases = lineas
            .SelectMany(l => (l.Pedidos.Count == 0 ? new List<string?> { null } : l.Pedidos.Cast<string?>().ToList())
                .Select(p => new Base(
                    l.ProveedorCode, l.ProveedorNombre, l.OrdenCode, l.Fecha, l.Moneda, l.ArticuloCode,
                    l.ArticuloNombre, l.Unidad, l.Cantidad, l.Entregada, l.Precio, l.Estado, p)))
            .ToList();

        var ordenes = bases.Select(b => b.OrdenCode).Distinct().ToList();
        var articulos = bases.Select(b => b.ArticuloCode).Distinct().ToList();

        // Notas de Ingreso procesadas de esas órdenes (legacy: codigo_est = '13').
        var ingresos = ordenes.Count == 0
            ? []
            : await _uow.Logistica.Transacciones.NotaIngresoDetalles.Query().AsNoTracking()
                .Where(i => i.PlantaCode == planta
                            && i.Estado == EstadoNotaIngresoDetalle.Procesado
                            && i.NotaIngreso.OrdenCompraCode != null
                            && ordenes.Contains(i.NotaIngreso.OrdenCompraCode!)
                            && articulos.Contains(i.ArticuloCode))
                .Select(i => new
                {
                    OrdenCode = i.NotaIngreso.OrdenCompraCode!,
                    NotaCode = i.NotaIngresoCode,
                    i.ArticuloCode, i.PedidoCode, i.GuiaCode,
                    i.Cantidad, i.Precio, i.Total
                })
                .ToListAsync(ct);

        var stocks = articulos.Count == 0
            ? new Dictionary<string, decimal>()
            : await _uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking()
                .Where(s => s.PlantaCode == planta && articulos.Contains(s.ArticuloCode))
                .ToDictionaryAsync(s => s.ArticuloCode, s => s.Quantity, ct);

        var filas = new List<ReporteSeguimientoOrdenCompraFilaResponse>();

        foreach (var b in bases)
        {
            // Mismo pedido; si la orden no tiene pedido de origen se toman los ingresos sin pedido.
            var nis = ingresos
                .Where(i => i.OrdenCode == b.OrdenCode && i.ArticuloCode == b.ArticuloCode && i.PedidoCode == b.PedidoCode)
                .OrderBy(i => i.NotaCode)
                .ToList();

            var estado = ReporteRules.Descripcion(b.Estado);
            estado = estado.Length > 3 ? estado[..3] : estado;
            stocks.TryGetValue(b.ArticuloCode, out var stock);

            if (nis.Count == 0)
            {
                filas.Add(Fila(b, planta, string.Empty, string.Empty, 0m, 0m, 0m, estado, stock));
                continue;
            }

            foreach (var n in nis)
                filas.Add(Fila(b, planta, n.GuiaCode ?? string.Empty, n.NotaCode, n.Cantidad, n.Precio, n.Total, estado, stock));
        }

        // Legacy: ORDER BY prv, com.
        var ordenadas = filas.OrderBy(f => f.ProveedorCode).ThenBy(f => f.OrdenCompraCode)
            .ThenBy(f => f.ArticuloCode).ThenBy(f => f.NotaIngresoCode).ToList();

        return new ReporteSeguimientoOrdenCompraResponse(planta, ordenadas.Count, ordenadas);
    }

    private static ReporteSeguimientoOrdenCompraFilaResponse Fila(
        Base b, string planta, string guia, string nota, decimal cantNi, decimal preNi, decimal totNi,
        string estado, decimal stock)
        => new(b.ProveedorCode, b.ProveedorNombre, planta, b.OrdenCode, b.Fecha, b.Moneda, b.ArticuloCode,
            b.ArticuloNombre, b.Unidad, b.Cantidad, b.Entregada, b.Precio, guia, nota, cantNi, preNi, totNi,
            estado, stock, b.PedidoCode);
}
