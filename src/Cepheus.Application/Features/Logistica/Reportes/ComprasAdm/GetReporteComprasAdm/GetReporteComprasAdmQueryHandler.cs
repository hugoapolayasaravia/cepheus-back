using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.ComprasAdm.GetReporteComprasAdm;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Compras_Adm.</summary>
public sealed class GetReporteComprasAdmQueryHandler : IRequestHandler<GetReporteComprasAdmQuery, ReporteComprasAdmResponse>
{
    private const string PlantaPorDefecto = "OF";

    private readonly IUnitOfWork _uow;

    public GetReporteComprasAdmQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteComprasAdmResponse> Handle(GetReporteComprasAdmQuery request, CancellationToken ct)
    {
        var planta = ReporteRules.Filtro(request.PlantaCode) ?? PlantaPorDefecto;
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);
        var proveedor = ReporteRules.Filtro(request.ProveedorCode);
        var comprobante = ReporteRules.Filtro(request.ComprobanteCode);
        var comprador = ReporteRules.Filtro(request.CompradorCode);

        var q = _uow.Logistica.Transacciones.OrdenServicioDetalles.Query().AsNoTracking()
            .Where(d => d.PlantaCode == planta
                        && d.OrdenServicio.FechaRecepcion >= desde
                        && d.OrdenServicio.FechaRecepcion < hastaExcl);

        if (proveedor is not null) q = q.Where(d => d.OrdenServicio.ProveedorCode == proveedor);
        if (comprobante is not null) q = q.Where(d => d.OrdenServicio.ComprobantePagoCode == comprobante);
        if (comprador is not null) q = q.Where(d => d.OrdenServicio.CompradorCode == comprador);
        if (request.EstadoCode.HasValue)
        {
            var e = (EstadoOrdenServicio)request.EstadoCode.Value;
            q = q.Where(d => d.OrdenServicio.Estado == e);
        }

        var datos = await q.Select(d => new
        {
            o = d.OrdenServicio,
            Comprobante = d.OrdenServicio.ComprobantePago.ShortName,
            ProveedorRuc = d.OrdenServicio.Proveedor.DocumentNumber,
            ProveedorNombre = d.OrdenServicio.Proveedor.LegalName,
            Comprador = d.OrdenServicio.Comprador.Name,
            FormaPago = d.OrdenServicio.FormaPago.Name,
            Dias = d.OrdenServicio.FormaPago.Days,
            d.ItemNumber, d.Glosa,
            d.Articulo.UnidadMedidaCode,
            d.Cantidad, d.Precio, d.Descuento, d.Total
        }).ToListAsync(ct);

        var tipoCambio = await CargarTiposCambioAsync(
            datos.Where(x => !TipoCambioRules.EsMonedaBase(x.o.MonedaCode))
                 .Select(x => (Moneda: x.o.MonedaCode.Trim().ToUpperInvariant(), Fecha: x.o.FechaRecepcion.Date))
                 .Distinct().ToList(), ct);

        var filas = datos
            .Select(x =>
            {
                // Moneda distinta de soles: tipo de cambio venta de la propia moneda del documento.
                var dolar = !TipoCambioRules.EsMonedaBase(x.o.MonedaCode);
                var tc = dolar ? Resolver(tipoCambio, x.o.MonedaCode.Trim().ToUpperInvariant(), x.o.FechaRecepcion.Date) : 1m;
                var anulada = x.o.Estado == EstadoOrdenServicio.Anulado;
                decimal Monto(decimal v) => anulada ? 0m : ReporteRules.ASoles(v, dolar, tc);

                return new ReporteCompraAdmFilaResponse(
                    x.o.Code, x.Comprobante, x.o.FechaRecepcion, x.o.ProveedorCode, x.ProveedorRuc, x.ProveedorNombre,
                    x.Comprador, x.o.MonedaCode, x.FormaPago, x.Dias, x.o.FechaRecepcion.AddDays(x.Dias),
                    ReporteRules.Descripcion(x.o.Estado),
                    Monto(x.o.Igv), Monto(x.o.Total), Monto(x.o.Monto), Monto(x.o.NoGravable),
                    // Legacy: la Renta no se anula con el estado.
                    ReporteRules.ASoles(x.o.Renta, dolar, tc),
                    x.ItemNumber, x.Glosa, x.UnidadMedidaCode, x.Cantidad,
                    Monto(x.Precio), x.Descuento, Monto(x.Total), x.o.NumeroDocumento);
            })
            .OrderBy(f => f.OrdenServicioCode).ThenBy(f => f.Item)
            .ToList();

        return new ReporteComprasAdmResponse(
            planta, desde, hastaExcl.AddDays(-1), filas.Count, filas.Sum(f => f.Total), filas);
    }

    /// <summary>Tipo de cambio venta por moneda y día; si un día no tiene, se usa el último anterior registrado.</summary>
    private async Task<Dictionary<string, SortedList<DateTime, decimal>>> CargarTiposCambioAsync(
        IReadOnlyList<(string Moneda, DateTime Fecha)> pares, CancellationToken ct)
    {
        var resultado = new Dictionary<string, SortedList<DateTime, decimal>>();
        if (pares.Count == 0) return resultado;

        var monedas = pares.Select(p => p.Moneda).Distinct().ToList();
        var min = DateOnly.FromDateTime(pares.Min(p => p.Fecha).AddDays(-15));
        var max = DateOnly.FromDateTime(pares.Max(p => p.Fecha));

        var tasas = await _uow.Comunes.TiposCambio.Query().AsNoTracking()
            .Where(t => monedas.Contains(t.MonedaCode) && t.Date >= min && t.Date <= max)
            .Select(t => new { t.MonedaCode, t.Date, t.SellRate })
            .ToListAsync(ct);

        foreach (var t in tasas)
        {
            if (!resultado.TryGetValue(t.MonedaCode, out var lista))
                resultado[t.MonedaCode] = lista = new SortedList<DateTime, decimal>();

            lista[t.Date.ToDateTime(TimeOnly.MinValue)] = t.SellRate;
        }

        return resultado;
    }

    private static decimal Resolver(Dictionary<string, SortedList<DateTime, decimal>> tasas, string moneda, DateTime fecha)
    {
        if (!tasas.TryGetValue(moneda, out var lista)) return 0m;

        decimal valor = 0m;
        foreach (var kv in lista)
        {
            if (kv.Key > fecha) break;
            valor = kv.Value;
        }
        return valor;
    }
}
