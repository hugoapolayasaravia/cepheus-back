using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.CreateOrdenServicio;

/// <summary>
/// La orden, sus líneas y su vale de salida se confirman con UN solo SaveChanges (una sola transacción). Crear NO mueve stock:
/// el stock y los materiales de la OT se tocan al Procesar.
/// </summary>
public sealed class CreateOrdenServicioCommandHandler
    : IRequestHandler<CreateOrdenServicioCommand, OrdenServicioResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateOrdenServicioCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenServicioResponse> Handle(CreateOrdenServicioCommand request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.PlantaCode);

        if (!await _uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == planta, ct))
            throw new KeyNotFoundException($"La planta {planta} no existe.");

        var comprobanteCode = OrdenServicioRules.Normalize(request.ComprobantePagoCode);
        var proveedor = OrdenServicioRules.Normalize(request.ProveedorCode);
        var moneda = OrdenServicioRules.Normalize(request.MonedaCode);
        var formaPago = OrdenServicioRules.Normalize(request.FormaPagoCode);
        var comprador = OrdenServicioRules.Normalize(request.CompradorCode);
        var lugarEnvio = OrdenServicioRules.Normalize(request.LugarEnvioCode);
        var tramite = OrdenServicioRules.Normalize(request.TramiteCode);
        var notaCompra = OrdenServicioRules.NormalizeOrNull(request.NotaCompraCode);
        var unidadNegocio = OrdenServicioRules.Normalize(request.UnidadNegocioCode);
        var trabajador = OrdenServicioRules.Normalize(request.TrabajadorCode);

        var comprobante = await OrdenServicioRules.ValidateHeaderAsync(
            _uow, comprobanteCode, proveedor, moneda, formaPago, comprador, lugarEnvio, tramite,
            notaCompra, unidadNegocio, trabajador, ct);

        // PB: la guía (09) no lleva número de documento; los demás sí.
        var esGuia = comprobante.Code == OrdenServicioRules.ComprobanteGuia;
        var numeroDocumento = esGuia ? null : request.NumeroDocumento?.Trim();

        if (!esGuia && string.IsNullOrWhiteSpace(numeroDocumento))
            throw new InvalidOperationException("Debe indicar el número de documento.");

        // Cierre de período, tope de 4 meses y fechas no mayores a la fecha de proceso (mismas reglas que Nota de Ingreso).
        await NotaIngresoRules.ValidateDatesAsync(_uow, planta, request.FechaEmision, request.FechaRecepcion, ct);

        if (!esGuia &&
            await OrdenServicioRules.ExistsDocumentoAsync(
                _uow, comprobante.Code, proveedor, numeroDocumento!, null, null, ct))
        {
            throw new InvalidOperationException(
                "Ya existe una Orden de Servicio con el mismo tipo de documento, proveedor y número.");
        }

        var tipoCambio = await NotaIngresoRules.ResolveTipoCambioAsync(_uow, moneda, request.FechaEmision, ct);

        var os = new OrdenServicio
        {
            PlantaCode = planta,
            ComprobantePagoCode = comprobante.Code,
            NumeroDocumento = numeroDocumento,
            ProveedorCode = proveedor,
            MonedaCode = moneda,
            FormaPagoCode = formaPago,
            FechaEmision = request.FechaEmision,
            FechaRecepcion = request.FechaRecepcion,
            TipoCambio = tipoCambio,
            Estado = EstadoOrdenServicio.Pendiente,
            CompradorCode = comprador,
            LugarEnvioCode = lugarEnvio,
            TramiteCode = tramite,
            NotaCompraCode = notaCompra,
            UnidadNegocioCode = unidadNegocio,
            Observaciones1 = string.IsNullOrWhiteSpace(request.Observaciones1) ? null : request.Observaciones1.Trim(),
            Observaciones2 = string.IsNullOrWhiteSpace(request.Observaciones2) ? null : request.Observaciones2.Trim()
        };

        var item = 0;
        foreach (var linea in request.Detalles)
        {
            var detalle = await OrdenServicioRules.BuildDetalleAsync(_uow, planta, linea, ct);
            detalle.ItemNumber = ++item;
            os.Detalles.Add(detalle);
        }

        await OrdenServicioTotalsCalculator.ApplyOnCreateAsync(_uow, os, comprobante, ct);

        os.Code = await OrdenServicioRules.NextCodeAsync(_uow, planta, ct);
        foreach (var detalle in os.Detalles)
            detalle.OrdenServicioCode = os.Code;

        // Vale de salida asociado (mismo número que la orden): responsable, fecha de entrega y líneas en soles.
        os.ValeSalidaCode = os.Code;
        os.ValeSalida = OrdenServicioSalidaSync.CreateFor(os, trabajador);
        await OrdenServicioSalidaSync.RecalculateAsync(_uow, os.ValeSalida, ct);

        await _uow.Logistica.Transacciones.OrdenesServicio.AddAsync(os, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            var detalleError = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"Error al guardar la Orden de Servicio: {detalleError}", ex);
        }

        return await OrdenServicioReader.GetAsync(_uow, planta, os.Code, ct);
    }
}
