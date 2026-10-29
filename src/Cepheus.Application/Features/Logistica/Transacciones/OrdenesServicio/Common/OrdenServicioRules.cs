using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Domain.Mantenimiento.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

/// <summary>
/// Reglas compartidas de la Orden de Servicio, traducidas del PowerBuilder (w_log_ordenservicio) y de los SP
/// Logi_sp_*. Las reglas que son idénticas a las de Nota de Ingreso (fechas, período cerrado, tipo de cambio,
/// conversión de precios, total de línea) delegan en NotaIngresoRules para tener una sola implementación.
/// </summary>
public static class OrdenServicioRules
{
    /// <summary>Código en TipoTransaccion para Orden de Servicio (el legacy envía 'OS' a la verificación de aprobación).</summary>
    public const string TipoTransaccionOS = "OS";

    public const string ComprobanteNotaCredito = NotaIngresoRules.ComprobanteNotaCredito;
    public const string ComprobanteGuia = NotaIngresoRules.ComprobanteGuia;

    /// <summary>Familia de artículos permitida (por ahora solo servicios, familia 50).</summary>
    public const string FamiliaServicio = "50";

    public const string TipoValeTransferencia = "005"; //"TRA";
    public const string TipoValeOrdenTrabajo = "COT";

    public const string CodePrefix = "6";
    public const int CorrelativeLength = 5;

    /// <summary>
    /// Sub centros de costo permitidos para el tipo de vale TRA (lista fija del PowerBuilder).
    /// Pendiente de confirmar si debe pasar a configuración.
    /// </summary>
    public static readonly HashSet<string> SubCentrosTransferencia = new()
    {
       "000000" //"210015", "690018", "817001", "821026", "832001", "110025", "823084", "710004"
    };

    public static string Normalize(string value) => NotaIngresoRules.Normalize(value);
    public static string? NormalizeOrNull(string? value) => NotaIngresoRules.NormalizeOrNull(value);

    // ---------------------------------------------------------------- correlativo

    /// <summary>Correlativo por planta con formato 6XXXXX.</summary>
    public static async Task<string> NextCodeAsync(IUnitOfWork uow, string plantaCode, CancellationToken ct)
    {
        var lastCode = await uow.Logistica.Transacciones.OrdenesServicio.Query()
            .AsNoTracking()
            .Where(x => x.PlantaCode == plantaCode && x.Code.StartsWith(CodePrefix))
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        var next = 1;
        if (lastCode is not null && int.TryParse(lastCode.Substring(CodePrefix.Length), out var last))
            next = last + 1;

        if (next > 99999)
            throw new InvalidOperationException(
                $"Se agotó el correlativo de Orden de Servicio para la planta {plantaCode}.");

        return CodePrefix + next.ToString().PadLeft(CorrelativeLength, '0');
    }

    // ---------------------------------------------------------------- estado legacy

    /// <summary>Acepta el código legacy (01, 09, 13, 11, 04) o el nombre del enum.</summary>
    public static bool TryParseEstado(string? value, out EstadoOrdenServicio estado)
    {
        switch (value?.Trim())
        {
            case "01": estado = EstadoOrdenServicio.Pendiente; return true;
            case "09": estado = EstadoOrdenServicio.Aprobado; return true;
            case "13": estado = EstadoOrdenServicio.Procesado; return true;
            case "11": estado = EstadoOrdenServicio.Cerrado; return true;
            case "04": estado = EstadoOrdenServicio.Anulado; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out estado)
                    && System.Enum.IsDefined(estado);
        }
    }

    // ---------------------------------------------------------------- cabecera

    /// <summary>
    /// Valida el comprobante y la existencia / vigencia de los maestros de la cabecera (el PowerBuilder lo hacía
    /// en cada itemchanged). Devuelve el comprobante para calcular los importes.
    /// </summary>
    public static async Task<ComprobantePago> ValidateHeaderAsync(
        IUnitOfWork uow,
        string comprobantePagoCode,
        string proveedorCode,
        string monedaCode,
        string formaPagoCode,
        string compradorCode,
        string lugarEnvioCode,
        string tramiteCode,
        string? notaCompraCode,
        string unidadNegocioCode,
        string trabajadorCode,
        CancellationToken ct)
    {
        var comprobante = await uow.Comunes.ComprobantesPago.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == comprobantePagoCode, ct);

        if (comprobante is null || !comprobante.IsActive || !comprobante.AvailableForPurchaseOrder)
            throw new InvalidOperationException("El comprobante de pago no existe o no admite Orden de Servicio.");

        // PB: tipo de documento '00' no es válido.
        if (comprobante.Code == "00")
            throw new InvalidOperationException("El tipo de documento no es válido.");

        // Nota de Crédito (07) sobre Orden de Servicio: flujo pendiente de migrar (fase 2).
        if (comprobante.Code == ComprobanteNotaCredito)
            throw new InvalidOperationException(
                "La Nota de Crédito sobre Orden de Servicio aún no está disponible.");

        if (!await uow.Logistica.Maestros.Proveedores.Query().AsNoTracking()
                .AnyAsync(p => p.Code == proveedorCode && p.IsActive, ct))
            throw new InvalidOperationException("El proveedor no existe o está inactivo.");

        if (!await uow.Comunes.Monedas.Query().AsNoTracking()
                .AnyAsync(m => m.Code == monedaCode && m.IsActive, ct))
            throw new InvalidOperationException("La moneda no existe o está inactiva.");

        if (!await uow.Logistica.Catalogos.FormasPago.Query().AsNoTracking()
                .AnyAsync(f => f.Code == formaPagoCode && f.IsActive, ct))
            throw new InvalidOperationException("La forma de pago no existe o está inactiva.");

        if (!await uow.Logistica.Catalogos.Compradores.Query().AsNoTracking()
                .AnyAsync(c => c.Code == compradorCode && c.IsActive, ct))
            throw new InvalidOperationException("El comprador no existe o está inactivo.");

        if (!await uow.Logistica.Catalogos.LugaresEnvio.Query().AsNoTracking()
                .AnyAsync(l => l.Code == lugarEnvioCode && l.IsActive, ct))
            throw new InvalidOperationException("El lugar de envío no existe o está inactivo.");

        if (!await uow.Logistica.Catalogos.Tramites.Query().AsNoTracking()
                .AnyAsync(t => t.Code == tramiteCode && t.IsActive, ct))
            throw new InvalidOperationException("El trámite (prioridad) no existe o está inactivo.");

        if (notaCompraCode is not null &&
            !await uow.Logistica.Catalogos.NotasCompra.Query().AsNoTracking()
                .AnyAsync(n => n.Code == notaCompraCode && n.IsActive, ct))
            throw new InvalidOperationException("La nota de la orden no existe o está inactiva.");

        if (!await uow.Logistica.Catalogos.UnidadesNegocio.Query().AsNoTracking()
                .AnyAsync(u => u.Code == unidadNegocioCode && u.IsActive, ct))
            throw new InvalidOperationException("La unidad de negocio no existe o está inactiva.");

        if (!await uow.Rrhh.Maestros.Trabajadores.Query().AsNoTracking()
                .AnyAsync(t => t.Code == trabajadorCode, ct))
            throw new InvalidOperationException("El trabajador responsable no existe.");

        return comprobante;
    }

    /// <summary>
    /// PB: no se repite (tipo de documento, proveedor, número) entre órdenes no anuladas.
    /// </summary>
    public static Task<bool> ExistsDocumentoAsync(
        IUnitOfWork uow,
        string comprobantePagoCode,
        string proveedorCode,
        string numeroDocumento,
        string? excludeCode,
        string? excludePlanta,
        CancellationToken ct)
        => uow.Logistica.Transacciones.OrdenesServicio.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.ComprobantePagoCode == comprobantePagoCode &&
                x.ProveedorCode == proveedorCode &&
                x.NumeroDocumento == numeroDocumento &&
                x.Estado != EstadoOrdenServicio.Anulado &&
                !(x.PlantaCode == excludePlanta && x.Code == excludeCode), ct);

    // ---------------------------------------------------------------- línea

    /// <summary>
    /// Valida una línea contra los maestros y arma la entidad (sin ItemNumber ni códigos de cabecera).
    /// Reglas del dw_5 / ue_set_btn_grb_data del PowerBuilder.
    /// </summary>
    public static async Task<OrdenServicioDetalle> BuildDetalleAsync(
        IUnitOfWork uow,
        string plantaCode,
        OrdenServicioDetalleRequest line,
        CancellationToken ct)
    {
        var articuloCode = Normalize(line.ArticuloCode);

        // PB: familia 50 (servicios).
        if (!articuloCode.StartsWith(FamiliaServicio, StringComparison.Ordinal))
            throw new InvalidOperationException(
                $"El artículo {articuloCode} no corresponde a una Orden de Servicio (solo familia {FamiliaServicio}).");

        var articulo = await uow.Logistica.Maestros.Articulos.Query().AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code == articuloCode, ct)
            ?? throw new InvalidOperationException($"El artículo {articuloCode} no existe.");

        if (!articulo.IsActive)
            throw new InvalidOperationException($"El artículo {articuloCode} está inactivo.");

        var total = NotaIngresoRules.LineTotal(line.Cantidad, line.Precio, line.Descuento);
        if (total <= 0)
            throw new InvalidOperationException(
                $"El total del artículo {articuloCode} debe ser mayor a cero.");

        var tipoValeCode = Normalize(line.TipoValeCode);
        if (!await uow.Logistica.Catalogos.TiposVale.Query().AsNoTracking()
                .AnyAsync(t => t.Code == tipoValeCode && t.IsActive, ct))
            throw new InvalidOperationException($"El tipo de vale {tipoValeCode} no existe o está inactivo.");

        var esCot = tipoValeCode == TipoValeOrdenTrabajo;
        var otCode = NormalizeOrNull(line.OrdenTrabajoCode);
        var sceCode = NormalizeOrNull(line.SubCentroEjecutorCode);
        var sccCode = NormalizeOrNull(line.SubCentroCostoCode);

        if (esCot)
        {
            // PB: tipo COT exige Orden de Trabajo en ejecución; de ella se toman sub centro de costo y ejecutor.
            if (otCode is null)
                throw new InvalidOperationException("Debe indicar la Orden de Trabajo para el tipo de vale COT.");

            var ot = await uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query().AsNoTracking()
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == otCode, ct)
                ?? throw new InvalidOperationException(
                    $"La Orden de Trabajo {otCode} no existe en la planta {plantaCode}.");

            if (ot.Estado != EstadoOrdenTrabajo.EnProceso)
                throw new InvalidOperationException(
                    "La Orden de Trabajo debe estar en estado de ejecución.");

            sccCode ??= ot.SubCentroCostoCode;
            sceCode ??= ot.SubCentroEjecutorCode;
        }
        else
        {
            if (otCode is not null)
                throw new InvalidOperationException(
                    "La Orden de Trabajo solo aplica al tipo de vale COT.");
            if (sceCode is not null)
                throw new InvalidOperationException(
                    "El sub centro ejecutor solo aplica al tipo de vale COT.");
        }

        if (sccCode is null)
            throw new InvalidOperationException("El sub centro de costo es obligatorio.");

        var scc = await uow.Logistica.Maestros.SubCentrosCosto.Query().AsNoTracking()
            .FirstOrDefaultAsync(s => s.Code == sccCode, ct)
            ?? throw new InvalidOperationException($"El sub centro de costo {sccCode} no existe.");

        if (!scc.IsActive)
            throw new InvalidOperationException($"El sub centro de costo {sccCode} está inactivo.");

        // PB: el tipo TRA solo admite una lista fija de sub centros.
        if (tipoValeCode == TipoValeTransferencia && !SubCentrosTransferencia.Contains(sccCode))
            throw new InvalidOperationException(
                "Sub centro de costo no válido para el tipo de vale TRA.");

        if (sceCode is not null &&
            !await uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query().AsNoTracking()
                .AnyAsync(s => s.Code == sceCode && s.IsActive, ct))
            throw new InvalidOperationException($"El sub centro ejecutor {sceCode} no existe o está inactivo.");

        return new OrdenServicioDetalle
        {
            PlantaCode = plantaCode,
            ArticuloCode = articuloCode,
            Glosa = string.IsNullOrWhiteSpace(line.Glosa) ? null : line.Glosa.Trim(),
            Cantidad = line.Cantidad,
            Precio = line.Precio,
            Descuento = line.Descuento,
            Total = total,
            Estado = EstadoOrdenServicioDetalle.Pendiente,
            TipoValeCode = tipoValeCode,
            SubCentroCostoCode = sccCode,
            SubCentroEjecutorCode = sceCode,
            OrdenTrabajoCode = otCode,
            // PB: la planta afectada es la del sub centro de costo.
            PlantaAfectadaCode = scc.PlantaCode
        };
    }

    /// <summary>Copia a una línea existente los campos editables (el artículo no se puede cambiar).</summary>
    public static void CopyEditableFields(OrdenServicioDetalle from, OrdenServicioDetalle to)
    {
        to.Glosa = from.Glosa;
        to.Cantidad = from.Cantidad;
        to.Precio = from.Precio;
        to.Descuento = from.Descuento;
        to.Total = from.Total;
        to.TipoValeCode = from.TipoValeCode;
        to.SubCentroCostoCode = from.SubCentroCostoCode;
        to.SubCentroEjecutorCode = from.SubCentroEjecutorCode;
        to.OrdenTrabajoCode = from.OrdenTrabajoCode;
        to.PlantaAfectadaCode = from.PlantaAfectadaCode;
    }
}
