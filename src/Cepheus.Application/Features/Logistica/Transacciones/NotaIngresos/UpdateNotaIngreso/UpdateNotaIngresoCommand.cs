using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.UpdateNotaIngreso;

/// <summary>
/// Modificación de la cabecera (PB, modo "M"): planta, condición, OC, proveedor, moneda, forma de pago y
/// líneas NO se pueden cambiar. Los importes (IGV, no gravable, renta, fonavi, servicio, IGV exterior)
/// solo aplican si el comprobante los habilita; los no habilitados quedan en 0.
/// </summary>
public sealed record UpdateNotaIngresoCommand(
    string PlantaCode,
    string Code,
    string ComprobantePagoCode,
    string? NumeroDocumento,
    string? NumeroGuia,
    DateTime FechaEmision,
    DateTime FechaRecepcion,
    decimal Igv,
    decimal NoGravable,
    decimal Renta,
    decimal Fonavi,
    decimal Servicio,
    decimal IgvExterior) : IRequest<NotaIngresoResponse>;

/// <summary>Cuerpo del PUT; la planta y el código vienen en la ruta.</summary>
public sealed record UpdateNotaIngresoRequest(
    string ComprobantePagoCode,
    string? NumeroDocumento,
    string? NumeroGuia,
    DateTime FechaEmision,
    DateTime FechaRecepcion,
    decimal Igv,
    decimal NoGravable,
    decimal Renta,
    decimal Fonavi,
    decimal Servicio,
    decimal IgvExterior);
