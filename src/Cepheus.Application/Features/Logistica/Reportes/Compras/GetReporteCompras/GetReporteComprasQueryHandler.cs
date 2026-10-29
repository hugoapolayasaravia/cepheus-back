using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.Compras.GetReporteCompras;

/// <summary>
/// Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Compras_cn_guia (que armaba el SELECT por
/// concatenación y ordenaba con un texto libre). Las conversiones a soles y el signo de la Nota de
/// Crédito se calculan igual que en el legacy.
/// </summary>
public sealed class GetReporteComprasQueryHandler : IRequestHandler<GetReporteComprasQuery, ReporteComprasResponse>
{
    public static readonly string[] Ordenamientos =
        { "NOTA_ING", "FECHA_EMI", "FECHA_ENT", "TIPO_DOC", "PROVEEDOR", "ARTICULO", "DOCUMENTO" };

    private readonly IUnitOfWork _uow;

    public GetReporteComprasQueryHandler(IUnitOfWork uow) => _uow = uow;

    private sealed record Cruda(
        string Atencion, string DocumentoCode, DateTime FechaEmision, DateTime FechaRecepcion,
        string ComprobanteCode, string ProveedorCode, string? ProveedorRuc, string? ProveedorNombre,
        string Estado, decimal Igv, decimal Total, decimal Monto, decimal NoGravable, decimal Renta,
        decimal Fonavi, decimal Servicio, decimal IgvExterior,
        string ArticuloCode, string ArticuloNombre, decimal Cantidad, decimal Precio, decimal TotalDetalle,
        string MonedaCode, decimal TipoCambio, string? NumeroDocumento, string? OrdenCompraCode, string? NumeroGuia);

    public async Task<ReporteComprasResponse> Handle(GetReporteComprasQuery request, CancellationToken ct)
    {
        var planta = ReporteRules.Normalizar(request.PlantaCode);
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);
        var proveedor = ReporteRules.Filtro(request.ProveedorCode);
        var comprobante = ReporteRules.Filtro(request.ComprobanteCode);
        var atencion = ReporteRules.Filtro(request.Atencion) ?? "T";

        var crudas = new List<Cruda>();

        if (atencion is "T" or "N")
        {
            var q = _uow.Logistica.Transacciones.NotaIngresoDetalles.Query().AsNoTracking()
                .Where(d => d.PlantaCode == planta
                            && d.Estado == EstadoNotaIngresoDetalle.Procesado
                            && d.NotaIngreso.FechaRecepcion >= desde
                            && d.NotaIngreso.FechaRecepcion < hastaExcl);

            if (proveedor is not null) q = q.Where(d => d.NotaIngreso.ProveedorCode == proveedor);
            if (comprobante is not null) q = q.Where(d => d.NotaIngreso.ComprobantePagoCode == comprobante);

            var filas = await q.Select(d => new
            {
                n = d.NotaIngreso,
                ProveedorRuc = d.NotaIngreso.Proveedor.DocumentNumber,
                ProveedorNombre = d.NotaIngreso.Proveedor.LegalName,
                d.ArticuloCode,
                ArticuloNombre = d.Articulo.Name,
                d.Cantidad, d.Precio, d.Total
            }).ToListAsync(ct);

            crudas.AddRange(filas.Select(f => new Cruda(
                "NI", f.n.Code, f.n.FechaEmision, f.n.FechaRecepcion, f.n.ComprobantePagoCode,
                f.n.ProveedorCode, f.ProveedorRuc, f.ProveedorNombre, ReporteRules.Descripcion(f.n.Estado),
                f.n.Igv, f.n.Total, f.n.Monto, f.n.NoGravable, f.n.Renta, f.n.Fonavi, f.n.Servicio, f.n.IgvExterior,
                f.ArticuloCode, f.ArticuloNombre, f.Cantidad, f.Precio, f.Total,
                f.n.MonedaCode, f.n.TipoCambio, f.n.NumeroDocumento, f.n.OrdenCompraCode, f.n.NumeroGuia)));
        }

        if (atencion is "T" or "S")
        {
            var q = _uow.Logistica.Transacciones.OrdenServicioDetalles.Query().AsNoTracking()
                .Where(d => d.PlantaCode == planta
                            && d.Estado == EstadoOrdenServicioDetalle.Procesado
                            && d.OrdenServicio.FechaRecepcion >= desde
                            && d.OrdenServicio.FechaRecepcion < hastaExcl);

            if (proveedor is not null) q = q.Where(d => d.OrdenServicio.ProveedorCode == proveedor);
            if (comprobante is not null) q = q.Where(d => d.OrdenServicio.ComprobantePagoCode == comprobante);

            var filas = await q.Select(d => new
            {
                o = d.OrdenServicio,
                ProveedorRuc = d.OrdenServicio.Proveedor.DocumentNumber,
                ProveedorNombre = d.OrdenServicio.Proveedor.LegalName,
                d.ArticuloCode,
                ArticuloNombre = d.Articulo.Name,
                d.Cantidad, d.Precio, d.Total
            }).ToListAsync(ct);

            crudas.AddRange(filas.Select(f => new Cruda(
                "OS", f.o.Code, f.o.FechaEmision, f.o.FechaRecepcion, f.o.ComprobantePagoCode,
                f.o.ProveedorCode, f.ProveedorRuc, f.ProveedorNombre, ReporteRules.Descripcion(f.o.Estado),
                f.o.Igv, f.o.Total, f.o.Monto, f.o.NoGravable, f.o.Renta, f.o.Fonavi, f.o.Servicio, f.o.IgvExterior,
                f.ArticuloCode, f.ArticuloNombre, f.Cantidad, f.Precio, f.Total,
                f.o.MonedaCode, f.o.TipoCambio, f.o.NumeroDocumento, null, null)));
        }

        var filasResp = crudas.Select(c => Mapear(c, planta)).ToList();
        var ordenadas = Ordenar(filasResp, ReporteRules.OrdenSeguro(request.OrdenarPor, Ordenamientos), request.Descendente);

        return new ReporteComprasResponse(
            planta, desde, hastaExcl.AddDays(-1), ordenadas.Count,
            ordenadas.Sum(f => f.TotalSoles), ordenadas);
    }

    private static ReporteCompraFilaResponse Mapear(Cruda c, string planta)
    {
        var dolar = ReporteRules.EsDolar(c.MonedaCode);
        var tc = c.TipoCambio;
        var signo = c.ComprobanteCode == ReporteRules.ComprobanteNotaCredito ? -1m : 1m;

        // TOTAL_NOI en dólares: redondeo por separado del neto y del IGV, como el legacy.
        var totalDocumento = dolar
            ? ReporteRules.Redondear(ReporteRules.ASoles(c.Total, true, tc) + ReporteRules.ASoles(c.Igv, true, tc))
            : ReporteRules.Redondear(c.Monto);

        return new ReporteCompraFilaResponse(
            planta, c.Atencion, c.DocumentoCode, c.FechaEmision, c.FechaRecepcion, c.ComprobanteCode,
            c.ProveedorCode, c.ProveedorRuc, c.ProveedorNombre, c.Estado,
            signo * ReporteRules.ASoles(c.Igv, dolar, tc),
            signo * ReporteRules.ASoles(c.Total, dolar, tc),
            signo * totalDocumento,
            signo * ReporteRules.ASoles(c.NoGravable, dolar, tc),
            signo * ReporteRules.ASoles(c.Renta, dolar, tc),
            signo * ReporteRules.ASoles(c.Fonavi, dolar, tc),
            signo * ReporteRules.ASoles(c.Servicio, dolar, tc),
            signo * ReporteRules.ASoles(c.IgvExterior, dolar, tc),
            c.ArticuloCode, c.ArticuloNombre, c.Cantidad,
            ReporteRules.ASoles(c.Precio, dolar, tc),
            c.TotalDetalle,
            ReporteRules.ASoles(c.TotalDetalle, dolar, tc),
            c.MonedaCode, tc, c.NumeroDocumento, c.OrdenCompraCode, c.NumeroGuia);
    }

    private static List<ReporteCompraFilaResponse> Ordenar(
        List<ReporteCompraFilaResponse> filas, string? orden, bool desc)
    {
        Func<ReporteCompraFilaResponse, object?> clave = (orden ?? "NOTA_ING").ToUpperInvariant() switch
        {
            "FECHA_EMI" => f => f.FechaEmision,
            "FECHA_ENT" => f => f.FechaRecepcion,
            "TIPO_DOC" => f => f.ComprobanteCode,
            "PROVEEDOR" => f => f.ProveedorNombre,
            "ARTICULO" => f => f.ArticuloCode,
            "DOCUMENTO" => f => f.NumeroDocumento,
            _ => f => f.DocumentoCode
        };

        var ord = desc ? filas.OrderByDescending(clave) : filas.OrderBy(clave);
        return ord.ThenBy(f => f.Atencion).ThenBy(f => f.DocumentoCode).ToList();
    }
}
