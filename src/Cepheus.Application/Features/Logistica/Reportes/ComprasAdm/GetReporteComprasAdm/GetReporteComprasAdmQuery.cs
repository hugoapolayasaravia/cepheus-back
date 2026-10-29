using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.ComprasAdm.GetReporteComprasAdm;

/// <summary>
/// Reporte de Compras Adm (legacy Logi_sp_Rpt_Compras_Adm): Órdenes de Servicio con fecha de pago, en soles.
/// El legacy tenía la planta fija en 'OF'; ahora es un parámetro y OF es el valor por defecto.
/// Las órdenes anuladas salen con importes en cero (excepto Renta), como en el legacy.
/// </summary>
/// <param name="TipoCambio">Los dólares se convierten con el tipo de cambio venta del día de recepción (ad_tipo_cambio), no con el guardado en el documento.</param>
public sealed record GetReporteComprasAdmQuery(
    string? PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? ProveedorCode,
    string? ComprobanteCode,
    string? CompradorCode,
    int? EstadoCode) : IRequest<ReporteComprasAdmResponse>;
