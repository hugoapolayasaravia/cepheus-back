using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.Compras.GetReporteCompras;

/// <summary>
/// Reporte de Compras (legacy Logi_sp_Rpt_Compras_cn_guia): detalle procesado de Notas de Ingreso y/o
/// Órdenes de Servicio por fecha de recepción. Importes en soles; las Notas de Crédito van en negativo.
/// </summary>
/// <param name="Atencion">null o T = Notas de Ingreso y Órdenes de Servicio; N = solo Notas de Ingreso; S = solo Órdenes de Servicio.</param>
/// <param name="OrdenarPor">NOTA_ING, FECHA_EMI, FECHA_ENT, TIPO_DOC, PROVEEDOR, ARTICULO o DOCUMENTO (por defecto NOTA_ING).</param>
public sealed record GetReporteComprasQuery(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? Atencion,
    string? ProveedorCode,
    string? ComprobanteCode,
    string? OrdenarPor,
    bool Descendente) : IRequest<ReporteComprasResponse>;
