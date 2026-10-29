// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/GenerarNotaIngresoImportacion/GenerarNotaIngresoImportacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.GenerarNotaIngresoImportacion
{
    /// <summary>
    /// Genera la Nota de Ingreso de las líneas Pendiente de UN proveedor de la importación e ingresa el stock
    /// (legacy: opción "1" de Generar, w_importaciones.ue_set_btn_grb_generar_ni).
    /// La moneda de la nota es PEN: el costo viene de ValorDet, que está en soles.
    /// NumeroDocumento no aplica si el comprobante es guía (09); NumeroReferencia es obligatorio si es 07.
    /// ArticuloCodes opcional: si no viene, se generan todas las líneas Pendiente del proveedor.
    /// </summary>
    public record GenerarNotaIngresoImportacionCommand(
        string PlantaCode,
        string ImportacionCode,
        string ProveedorCode,
        string ComprobantePagoCode,
        string? NumeroDocumento,
        string? NumeroGuia,
        string? NumeroReferencia,
        string? ComprobantePagoReferenciaCode,
        string FormaPagoCode,
        DateTime FechaEmision,
        DateTime FechaRecepcion,
        IReadOnlyList<string>? ArticuloCodes
    ) : IRequest<NotaIngresoResponse>;
}
