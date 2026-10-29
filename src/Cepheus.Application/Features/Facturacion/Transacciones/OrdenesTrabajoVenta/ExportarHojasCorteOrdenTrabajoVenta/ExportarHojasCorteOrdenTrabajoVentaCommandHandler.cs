using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ExportarHojasCorteOrdenTrabajoVenta;

public sealed class ExportarHojasCorteOrdenTrabajoVentaCommandHandler
    : IRequestHandler<ExportarHojasCorteOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public ExportarHojasCorteOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(ExportarHojasCorteOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureEditable(ot);

        if (ot.Type == TipoOrdenTrabajoVenta.Descuento)
        {
            throw new InvalidOperationException("Una OT Descuento no admite Hojas de Corte.");
        }

        var pt = OrdenTrabajoVentaRules.NegocioPretensados;

        var ptOk = await _uow.Comunes.Negocios.Query().AsNoTracking().AnyAsync(n => n.Code == pt && n.IsActive, ct);

        if (!ptOk)
        {
            throw new KeyNotFoundException($"El negocio {pt} no existe o está inactivo.");
        }

        // Hojas de Corte solicitadas (sin repetir), tracked para vincularlas.
        var refs = request.Data.Hojas
            .Select(h => (Year: OrdenTrabajoVentaRules.Normalize(h.Year), Code: OrdenTrabajoVentaRules.NormalizeCode(h.Code)))
            .Distinct()
            .ToList();

        var hojas = new List<HojaCorte>();

        foreach (var (year, hcCode) in refs)
        {
            var hc = await _uow.Facturacion.Transacciones.HojasCorte
                .Query()
                .Include(x => x.Detalles)
                .FirstOrDefaultAsync(x => x.Year == year && x.Code == hcCode, ct)
                ?? throw new KeyNotFoundException($"La Hoja de Corte {year}/{hcCode} no existe.");

            if (hc.Status != EstadoHojaCorte.Pendiente || hc.OrdenTrabajoCode is not null)
            {
                throw new InvalidOperationException(
                    $"La Hoja de Corte {year}/{hcCode} está {hc.Status}" +
                    (hc.OrdenTrabajoCode is null ? "." : $" y ya pertenece a la OT {hc.OrdenTrabajoCode}.") +
                    " Solo se exportan Hojas de Corte Pendientes sin OT.");
            }

            if (hc.ClienteCode != ot.ClienteCode || hc.ObraCode != ot.ObraCode)
            {
                throw new InvalidOperationException(
                    $"La Hoja de Corte {year}/{hcCode} no es del cliente/obra de la OT ({ot.ClienteCode}/{ot.ObraCode}).");
            }

            if (hc.Detalles.Count == 0)
            {
                throw new InvalidOperationException($"La Hoja de Corte {year}/{hcCode} no tiene líneas.");
            }

            hojas.Add(hc);
        }

        // Una línea por producto: vigueta = metros lineales totales, bovedilla = cantidad.
        var grouped = hojas
            .SelectMany(h => h.Detalles)
            .GroupBy(d => (d.ProductoTipoCode, d.ProductoCode))
            .Select(g => new
            {
                Tipo = g.Key.ProductoTipoCode,
                Producto = g.Key.ProductoCode,
                Quantity = g.Key.ProductoTipoCode == HojaCorteProductoTipos.Vigueta
                    ? g.Sum(d => d.TotalLength)
                    : g.Sum(d => d.Quantity)
            })
            .OrderBy(x => x.Tipo).ThenBy(x => x.Producto, StringComparer.Ordinal)
            .ToList();

        var precios = request.Data.Precios
            .GroupBy(p => (Tipo: OrdenTrabajoVentaLineas.NormalizeTipo(p.ProductoTipoCode),
                           Producto: OrdenTrabajoVentaLineas.NormalizeProducto(p.ProductoCode)))
            .ToDictionary(g => g.Key, g => g.Last().UnitPrice);

        var faltantes = grouped
            .Where(g => !precios.ContainsKey((g.Tipo, g.Producto)))
            .Select(g => $"{g.Tipo}/{g.Producto}")
            .ToList();

        if (faltantes.Count > 0)
        {
            throw new InvalidOperationException(
                "Falta el precio unitario de: " + string.Join(", ", faltantes) + ".");
        }

        var ctx = await OrdenTrabajoVentaLineas.BuildContextAsync(_uow, ot, ct);

        foreach (var g in grouped)
        {
            // Las líneas de Hoja de Corte no pasan por la regla "no agregar a mano": se agregan aquí.
            await OrdenTrabajoVentaLineas.AddAsync(
                _uow,
                new LineContext { Ot = ot, Cotizacion = ctx.Cotizacion, HasHojasCorte = false, OriginNegocios = ctx.OriginNegocios },
                new OrdenTrabajoVentaDetalleRequest
                {
                    ProductoTipoCode = g.Tipo,
                    ProductoCode = g.Producto,
                    NegocioCode = pt,
                    Quantity = g.Quantity,
                    UnitPrice = precios[(g.Tipo, g.Producto)]
                },
                ct);
        }

        var notas = new List<string>();

        foreach (var hc in hojas.OrderBy(h => h.Year).ThenBy(h => h.Code, StringComparer.Ordinal))
        {
            hc.Status = EstadoHojaCorte.Procesado;
            hc.OrdenTrabajoCode = ot.Code;

            var number = int.TryParse(hc.Code, NumberStyles.None, CultureInfo.InvariantCulture, out var n) ? n.ToString(CultureInfo.InvariantCulture) : hc.Code;
            notas.Add($"HC {number}/{hc.Year} Nivel: {hc.Floor} Sector: {hc.Sector}");
        }

        ot.CutSheetObservations = string.IsNullOrWhiteSpace(ot.CutSheetObservations)
            ? string.Join(Environment.NewLine, notas)
            : ot.CutSheetObservations + Environment.NewLine + string.Join(Environment.NewLine, notas);

        OrdenTrabajoVentaRules.Recalculate(ot);

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
