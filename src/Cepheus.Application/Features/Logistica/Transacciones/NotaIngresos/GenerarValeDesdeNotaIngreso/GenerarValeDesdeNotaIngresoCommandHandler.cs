using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GenerarValeDesdeNotaIngreso;

/// <summary>
/// ue_vale del Maestro de Nota de Ingresos (legacy). Reutiliza las reglas del módulo de Vales (cabecera, línea, IGV por
/// fecha, correlativo) para que este vale se comporte igual que uno creado a mano. Nota y vale se confirman con UN solo
/// SaveChanges.
///   1. Solo una nota procesada y que todavía no generó un vale.
///   2. Regla del saldo (dw_con_stocks_ni_negativos_vales): si alguna línea supera Stock − Pendiente, se detiene.
///   3. Vale tipo Transferencia, Pendiente, con las líneas de la nota (precio en soles, sin descuento, sin horómetro).
///   4. La nota queda enlazada con el vale (CodigoVale); desde entonces no se puede anular. El script legacy no escribe
///      ese enlace en ningún punto del botón; se graba aquí porque la regla de anulación lo supone.
/// El stock no se mueve: el vale de transferencia mueve el stock cuando el almacén destino lo recibe.
/// </summary>
public sealed class GenerarValeDesdeNotaIngresoCommandHandler
    : IRequestHandler<GenerarValeDesdeNotaIngresoCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;

    public GenerarValeDesdeNotaIngresoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ValeResponse> Handle(GenerarValeDesdeNotaIngresoCommand request, CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.PlantaCode);
        var code = NotaIngresoRules.Normalize(request.NotaIngresoCode);

        var note = await _uow.Logistica.Transacciones.NotaIngresos.Query()
            .Include(n => n.Detalles)
            .FirstOrDefaultAsync(n => n.PlantaCode == planta && n.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Nota de Ingreso {planta}/{code} no existe.");

        if (note.Estado != EstadoNotaIngreso.Procesado)
            throw new InvalidOperationException("Solo se puede generar un vale desde una Nota de Ingreso procesada.");

        if (!string.IsNullOrWhiteSpace(note.CodigoVale))
            throw new InvalidOperationException($"La Nota de Ingreso ya generó el vale {note.CodigoVale}.");

        var lineas = note.Detalles
            .Where(d => d.Estado == EstadoNotaIngresoDetalle.Procesado)
            .OrderBy(d => d.ItemNumber)
            .ToList();

        if (lineas.Count == 0)
            throw new InvalidOperationException("La Nota de Ingreso no tiene líneas procesadas.");

        // ------------------------------------------------------------ regla del saldo
        var sinStock = await NotaIngresoValeRules.GetLineasSinStockAsync(_uow, planta, lineas, ct);
        if (sinStock.Count > 0)
        {
            var detalle = string.Join("; ", sinStock.Select(s =>
                $"{s.ArticuloCode} (cantidad {s.Cantidad:0.##}, stock {s.Stock:0.##}, pendiente {s.Pendiente:0.##})"));

            throw new InvalidOperationException(
                "Existen artículos sin saldo suficiente para generar el vale: " + detalle + ".");
        }

        // ------------------------------------------------------------ cabecera del vale (tipo Transferencia)
        var cabecera = await ValeCabecera.ResolverAsync(
            _uow,
            planta,
            ValeRules.TipoValeTransferencia,
            request.FechaEntrega,
            request.SubCentroCostoCode,
            null,
            request.TrabajadorCode,
            request.UnidadNegocioCode,
            request.PlantaAfectadaCode,
            null,
            null,
            ct);

        var vale = new Vale
        {
            PlantaCode = planta,
            TipoValeCode = cabecera.TipoValeCode,
            FechaEntrega = cabecera.FechaEntrega,
            SubCentroCostoCode = cabecera.SubCentroCostoCode,
            SubCentroEjecutorCode = cabecera.SubCentroEjecutorCode,
            TrabajadorCode = cabecera.TrabajadorCode,
            OrdenTrabajoCode = null,
            UnidadNegocioCode = cabecera.UnidadNegocioCode,
            PlantaAfectadaCode = cabecera.PlantaAfectadaCode,
            Estado = EstadoVale.Pendiente
        };

        // ------------------------------------------------------------ líneas: las de la nota, precio en soles sin descuento
        var esDolar = note.MonedaCode == NotaIngresoRules.MonedaDolar;

        var lineasVale = ValeDesdeNotaCalculo.AgruparPorArticulo(
            lineas.Select(l => new LineaNota(
                l.ArticuloCode,
                l.Cantidad,
                ValeDesdeNotaCalculo.PrecioSoles(l.Precio, esDolar, note.TipoCambio))));

        foreach (var linea in lineasVale)
        {
            var resuelta = await ValeLineas.ResolverAsync(
                _uow,
                planta,
                cabecera.TipoValeCode,
                linea.ArticuloCode,
                linea.Cantidad,
                null,
                0m,
                linea.Precio,
                false,
                ct);

            vale.Detalles.Add(new ValeDetalle
            {
                PlantaCode = planta,
                ArticuloCode = linea.ArticuloCode,
                Cantidad = linea.Cantidad,
                Precio = resuelta.Precio,
                Total = ValeRules.LineTotal(linea.Cantidad, resuelta.Precio),
                Estado = EstadoValeDetalle.Pendiente,
                Propiedad01 = resuelta.Propiedad01
            });
        }

        ValeLineas.Renumerar(vale);
        await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);

        vale.Code = await ValeRules.NextCodeAsync(_uow, planta, ct);

        foreach (var detalle in vale.Detalles)
            detalle.ValeCode = vale.Code;

        // ------------------------------------------------------------ enlace y guardado (una sola transacción)
        note.CodigoVale = vale.Code;

        await _uow.Logistica.Transacciones.Vales.AddAsync(vale, ct);
        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, vale.Code, ct);
    }
}
