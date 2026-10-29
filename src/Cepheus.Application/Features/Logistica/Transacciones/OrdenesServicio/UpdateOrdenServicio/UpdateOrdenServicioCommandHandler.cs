using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.UpdateOrdenServicio;

public sealed class UpdateOrdenServicioCommandHandler
    : IRequestHandler<UpdateOrdenServicioCommand, OrdenServicioResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateOrdenServicioCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenServicioResponse> Handle(UpdateOrdenServicioCommand request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.PlantaCode);
        var code = OrdenServicioRules.Normalize(request.Code);

        var os = await _uow.Logistica.Transacciones.OrdenesServicio.Query()
            .Include(x => x.Detalles)
            .Include(x => x.ValeSalida).ThenInclude(v => v.Detalles)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Orden de Servicio {planta}/{code} no existe.");

        // PB (ue_set_btn_mod_dw): anulada o cerrada no se modifica; con asiento tampoco; y el período debe estar abierto.
        if (os.Estado is EstadoOrdenServicio.Anulado or EstadoOrdenServicio.Cerrado)
            throw new InvalidOperationException(
                $"La Orden de Servicio está en estado '{os.Estado}' y no admite modificación.");

        if (!string.IsNullOrWhiteSpace(os.AsientoContable))
            throw new InvalidOperationException(
                "La Orden de Servicio ya generó asiento contable. Imposible su modificación.");

        await NotaIngresoRules.EnsurePeriodOpenAsync(
            _uow, planta, os.FechaRecepcion,
            "La Orden de Servicio está en un período cerrado. Imposible su modificación.", ct);

        var comprobanteCode = OrdenServicioRules.Normalize(request.ComprobantePagoCode);
        var proveedor = OrdenServicioRules.Normalize(request.ProveedorCode);
        var moneda = OrdenServicioRules.Normalize(request.MonedaCode);
        var formaPago = OrdenServicioRules.Normalize(request.FormaPagoCode);

        // Aprobado / Procesado: comprobante, proveedor, moneda y forma de pago bloqueados (TabSequence = 0 en el PB).
        if (os.Estado != EstadoOrdenServicio.Pendiente &&
            (comprobanteCode != os.ComprobantePagoCode ||
             proveedor != os.ProveedorCode ||
             moneda != os.MonedaCode ||
             formaPago != os.FormaPagoCode))
        {
            throw new InvalidOperationException(
                $"En estado '{os.Estado}' no se puede cambiar el comprobante, el proveedor, la moneda ni la forma de pago.");
        }

        var comprador = OrdenServicioRules.Normalize(request.CompradorCode);
        var lugarEnvio = OrdenServicioRules.Normalize(request.LugarEnvioCode);
        var tramite = OrdenServicioRules.Normalize(request.TramiteCode);
        var notaCompra = OrdenServicioRules.NormalizeOrNull(request.NotaCompraCode);
        var unidadNegocio = OrdenServicioRules.Normalize(request.UnidadNegocioCode);
        var trabajador = OrdenServicioRules.Normalize(request.TrabajadorCode);

        var comprobante = await OrdenServicioRules.ValidateHeaderAsync(
            _uow, comprobanteCode, proveedor, moneda, formaPago, comprador, lugarEnvio, tramite,
            notaCompra, unidadNegocio, trabajador, ct);

        var esGuia = comprobante.Code == OrdenServicioRules.ComprobanteGuia;
        var numeroDocumento = esGuia ? null : request.NumeroDocumento?.Trim();

        if (!esGuia && string.IsNullOrWhiteSpace(numeroDocumento))
            throw new InvalidOperationException("Debe indicar el número de documento.");

        await NotaIngresoRules.ValidateDatesAsync(_uow, planta, request.FechaEmision, request.FechaRecepcion, ct);

        var cambioDocumento =
            comprobante.Code != os.ComprobantePagoCode ||
            proveedor != os.ProveedorCode ||
            numeroDocumento != os.NumeroDocumento;

        if (!esGuia && cambioDocumento &&
            await OrdenServicioRules.ExistsDocumentoAsync(
                _uow, comprobante.Code, proveedor, numeroDocumento!, os.Code, os.PlantaCode, ct))
        {
            throw new InvalidOperationException(
                "Ya existe una Orden de Servicio con el mismo tipo de documento, proveedor y número.");
        }

        // El tipo de cambio depende de la moneda y de la fecha de emisión.
        if (moneda != os.MonedaCode || request.FechaEmision.Date != os.FechaEmision.Date)
            os.TipoCambio = await NotaIngresoRules.ResolveTipoCambioAsync(_uow, moneda, request.FechaEmision, ct);

        var comprobanteCambio = comprobante.Code != os.ComprobantePagoCode;

        os.ComprobantePagoCode = comprobante.Code;
        os.NumeroDocumento = numeroDocumento;
        os.ProveedorCode = proveedor;
        os.MonedaCode = moneda;
        os.FormaPagoCode = formaPago;
        os.FechaEmision = request.FechaEmision;
        os.FechaRecepcion = request.FechaRecepcion;
        os.CompradorCode = comprador;
        os.LugarEnvioCode = lugarEnvio;
        os.TramiteCode = tramite;
        os.NotaCompraCode = notaCompra;
        os.UnidadNegocioCode = unidadNegocio;
        os.ValeSalida.TrabajadorCode = trabajador;
        os.ValeSalida.FechaEntrega = os.FechaRecepcion;
        os.Observaciones1 = string.IsNullOrWhiteSpace(request.Observaciones1) ? null : request.Observaciones1.Trim();
        os.Observaciones2 = string.IsNullOrWhiteSpace(request.Observaciones2) ? null : request.Observaciones2.Trim();

        // Mientras la orden está Pendiente el vale refleja la orden (el tipo de cambio puede haber cambiado).
        if (os.Estado == EstadoOrdenServicio.Pendiente)
        {
            OrdenServicioSalidaSync.SyncLines(_uow, os);
            await OrdenServicioSalidaSync.RecalculateAsync(_uow, os.ValeSalida, ct);
        }

        await OrdenServicioTotalsCalculator.ApplyOnUpdateAsync(
            _uow, os, comprobante, comprobanteCambio,
            request.Igv, request.NoGravable, request.Renta, request.Fonavi, request.Servicio, request.IgvExterior, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "La Orden de Servicio fue modificada por otro usuario. Recargue los datos e intente nuevamente.");
        }
        catch (DbUpdateException ex)
        {
            var detalleError = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"Error al guardar la Orden de Servicio: {detalleError}", ex);
        }

        return await OrdenServicioReader.GetAsync(_uow, planta, code, ct);
    }
}
