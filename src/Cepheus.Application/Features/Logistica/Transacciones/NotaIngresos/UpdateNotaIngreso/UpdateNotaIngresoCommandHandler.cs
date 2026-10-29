using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.UpdateNotaIngreso;

public sealed class UpdateNotaIngresoCommandHandler
    : IRequestHandler<UpdateNotaIngresoCommand, NotaIngresoResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateNotaIngresoCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<NotaIngresoResponse> Handle(UpdateNotaIngresoCommand request, CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.PlantaCode);
        var code = NotaIngresoRules.Normalize(request.Code);

        var note = await _uow.Logistica.Transacciones.NotaIngresos.Query()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Nota de Ingreso {planta}/{code} no existe.");

        if (note.Estado == EstadoNotaIngreso.Anulado)
            throw new InvalidOperationException("La Nota de Ingreso está anulada y no se puede modificar.");

        if (note.Origen == OrigenNotaIngreso.Transferencia)
            throw new InvalidOperationException(
                "Una Nota de Ingreso generada por una transferencia no se puede modificar: su costo lo fija el cierre del almacén origen.");

        if (!string.IsNullOrWhiteSpace(note.AsientoContable))
            throw new InvalidOperationException(
                "La Nota de Ingreso ya tiene asiento contable y no se puede modificar.");

        // PB: no se modifica si su período (con la fecha de recepción actual) ya está cerrado.
        await NotaIngresoRules.EnsurePeriodOpenAsync(
            _uow, planta, note.FechaRecepcion,
            "La Nota de Ingreso pertenece a un período cerrado y no se puede modificar.", ct);

        var actual = await _uow.Comunes.ComprobantesPago.Query()
            .AsNoTracking()
            .FirstAsync(c => c.Code == note.ComprobantePagoCode, ct);

        var nuevo = await _uow.Comunes.ComprobantesPago.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == request.ComprobantePagoCode, ct);

        if (nuevo is null || !nuevo.IsActive || !nuevo.AvailableForPurchaseOrder)
            throw new InvalidOperationException(
                "El comprobante de pago no existe o no admite Nota de Ingreso.");

        var eraNotaCredito = actual.Code == NotaIngresoRules.ComprobanteNotaCredito;
        var esNotaCredito = nuevo.Code == NotaIngresoRules.ComprobanteNotaCredito;
        if (eraNotaCredito != esNotaCredito)
            throw new InvalidOperationException(
                "No se puede cambiar el comprobante hacia o desde una Nota de Crédito.");

        var esGuia = nuevo.Code == NotaIngresoRules.ComprobanteGuia;
        var numeroDocumento = esGuia ? null : request.NumeroDocumento?.Trim();
        var numeroGuia = request.NumeroGuia?.Trim();

        if (!esGuia && string.IsNullOrWhiteSpace(numeroDocumento))
            throw new InvalidOperationException("Debe indicar el número de documento.");
        if (esGuia && string.IsNullOrWhiteSpace(numeroGuia))
            throw new InvalidOperationException("Debe indicar el número de guía.");

        await NotaIngresoRules.ValidateDatesAsync(_uow, planta, request.FechaEmision, request.FechaRecepcion, ct);

        // PB: el duplicado solo se revisa cuando cambia el número de documento (o el tipo).
        var comprobanteChanged = nuevo.Code != actual.Code;
        if (!esGuia && (comprobanteChanged || !string.Equals(numeroDocumento, note.NumeroDocumento, StringComparison.Ordinal)))
        {
            var duplicado = await _uow.Logistica.Transacciones.NotaIngresos.Query()
                .AsNoTracking()
                .AnyAsync(x => x.ComprobantePagoCode == nuevo.Code
                               && x.ProveedorCode == note.ProveedorCode
                               && x.NumeroDocumento == numeroDocumento
                               && x.Estado != EstadoNotaIngreso.Anulado
                               && !(x.PlantaCode == planta && x.Code == code), ct);

            if (duplicado)
                throw new InvalidOperationException(
                    "Ya existe una Nota de Ingreso con el mismo tipo de documento, proveedor y número.");
        }

        // El tipo de cambio sigue a la fecha de emisión (solo dólares y no Nota de Crédito).
        if (!esNotaCredito
            && NotaIngresoRules.EsDolar(note.MonedaCode)
            && request.FechaEmision.Date != note.FechaEmision.Date)
        {
            note.TipoCambio = await NotaIngresoRules.ResolveTipoCambioAsync(
                _uow, note.MonedaCode, request.FechaEmision, ct);
        }

        note.ComprobantePagoCode = nuevo.Code;
        note.NumeroDocumento = numeroDocumento;
        note.NumeroGuia = esNotaCredito ? null : numeroGuia;
        note.FechaEmision = request.FechaEmision;
        note.FechaRecepcion = request.FechaRecepcion;
        if (!esNotaCredito)
            note.FechaDocumento = request.FechaEmision;

        await NotaIngresoTotalsCalculator.ApplyOnUpdateAsync(
            _uow, note, nuevo, comprobanteChanged,
            request.Igv, request.NoGravable, request.Renta, request.Fonavi, request.Servicio, request.IgvExterior,
            ct);

        _uow.Logistica.Transacciones.NotaIngresos.Update(note);
        await _uow.SaveChangesAsync(ct);

        return await NotaIngresoReader.GetAsync(_uow, planta, code, ct);
    }
}
