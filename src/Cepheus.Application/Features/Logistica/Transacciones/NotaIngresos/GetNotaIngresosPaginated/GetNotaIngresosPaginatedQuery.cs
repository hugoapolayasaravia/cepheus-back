using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresosPaginated;

/// <summary>
/// Parámetros literales de Logi_sp_Listado_MNotaIngresos:
///   Codigo_Pla (obligatorio) | Codigo_NoI ('T' = todas, o un código) | Codigo_Prv ('T' o código de proveedor)
///   Codigo_Est ('T' o estado: 01, 13, 11, 04 — también se acepta el nombre) | Fecha_Ini / Fecha_Fin
///   (obligatorias cuando Codigo_NoI = 'T'; filtran por fecha de proceso) | Pago ('T' o código de forma de pago).
/// Un parámetro omitido equivale a 'T'.
/// </summary>
public sealed class GetNotaIngresosPaginatedQuery : PagedRequest, IRequest<PagedResult<NotaIngresoListadoResponse>>
{
    public string? Codigo_Pla { get; set; }
    public string? Codigo_NoI { get; set; }
    public string? Codigo_Prv { get; set; }
    public string? Codigo_Est { get; set; }
    public DateTime? Fecha_Ini { get; set; }
    public DateTime? Fecha_Fin { get; set; }
    public string? Pago { get; set; }
}
