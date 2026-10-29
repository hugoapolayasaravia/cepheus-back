using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.Pedidos.GetReportePedidosDetalle;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Pedidos_Detalle.</summary>
public sealed class GetReportePedidosDetalleQueryHandler
    : IRequestHandler<GetReportePedidosDetalleQuery, ReportePedidosDetalleResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReportePedidosDetalleQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReportePedidosDetalleResponse> Handle(GetReportePedidosDetalleQuery request, CancellationToken ct)
    {
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);
        var planta = ReporteRules.Filtro(request.PlantaCode);
        var scc = ReporteRules.Filtro(request.SubCentroCostoCode);
        var tipo = ReporteRules.Filtro(request.TipoPedidoCode);
        var usuario = string.IsNullOrWhiteSpace(request.Usuario) || ReporteRules.EsTodos(request.Usuario)
            ? null
            : request.Usuario.Trim();

        var q = _uow.Logistica.Transacciones.Pedidos.Query().AsNoTracking()
            .Where(p => p.CreatedAt >= desde && p.CreatedAt < hastaExcl);

        if (planta is not null) q = q.Where(p => p.PlantaCode == planta);
        if (scc is not null) q = q.Where(p => p.SubCentroCostoCode == scc);
        if (tipo is not null) q = q.Where(p => p.TipoPedidoCode == tipo);
        if (usuario is not null) q = q.Where(p => p.CreatedBy == usuario);
        if (request.EstadoCode.HasValue)
        {
            var e = (EstadoPedido)request.EstadoCode.Value;
            q = q.Where(p => p.EstadoPedido == e);
        }

        // LEFT JOIN al detalle, como el legacy: un pedido sin detalle sale en una fila con los campos del artículo vacíos.
        var datos = await q
            .SelectMany(p => p.Detalles.DefaultIfEmpty(), (p, d) => new
            {
                p.PlantaCode,
                p.Code,
                TipoPedidoNombre = p.TipoPedido.Name,
                p.CreatedAt,
                p.FechaEntrega,
                TramiteNombre = p.Tramite.Name,
                SubCentroNombre = p.SubCentroCosto.Name,
                p.OrdenTrabajoCode,
                p.Observaciones,
                p.CreatedBy,
                p.AprobadoPor,
                p.EstadoPedido,
                ArticuloCode = d != null ? d.ArticuloCode : null,
                Descripcion = d != null ? d.DescripcionArticulo : null,
                OrdenCompraCode = d != null ? d.OrdenCompraCode : null,
                Unidad = d != null ? d.UnidadMedidaCode : null,
                Cantidad = d != null ? (decimal?)d.CantidadArticulo : null,
                EstadoDetalle = d != null ? (EstadoPedidoDetalle?)d.EstadoPedidoDetalle : null
            })
            .ToListAsync(ct);

        // Legacy: ORDER BY 1, 2 desc (planta y pedido).
        var filas = datos
            .OrderBy(x => x.PlantaCode).ThenByDescending(x => x.Code).ThenBy(x => x.ArticuloCode)
            .Select(x => new ReportePedidoDetalleFilaResponse(
                x.PlantaCode, x.Code, x.TipoPedidoNombre, x.CreatedAt, x.FechaEntrega, x.TramiteNombre,
                x.SubCentroNombre, x.OrdenTrabajoCode, x.Observaciones, x.CreatedBy, x.AprobadoPor,
                ReporteRules.Descripcion(x.EstadoPedido), x.ArticuloCode, x.Descripcion, x.Unidad, x.OrdenCompraCode, x.Cantidad,
                x.EstadoDetalle.HasValue ? ReporteRules.Descripcion(x.EstadoDetalle.Value) : null))
            .ToList();

        return new ReportePedidosDetalleResponse(desde, hastaExcl.AddDays(-1), filas.Count, filas);
    }
}
