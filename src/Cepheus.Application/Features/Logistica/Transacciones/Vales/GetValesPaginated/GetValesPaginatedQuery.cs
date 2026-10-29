using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValesPaginated;

/// <summary>
/// Parámetros literales de Logi_sp_Listado_MVales:
///   Codigo_Pla (obligatorio) | Codigo_Val ('T' = todos, o un código)
///   Codigo_Est ('T' o estado: 01, 09, 12, 13, 14, 30, 04 — también se acepta el nombre)
///   Codigo_Usu ('T' o texto contenido en el usuario que lo registró)
///   Fecha_Ini / Fecha_Fin (filtran por fecha de proceso; opcionales)
///   Responsable ('T' o código de trabajador).
/// Los filtros de estado, usuario, fechas y responsable solo aplican con Codigo_Val = 'T', como el SP.
/// Un parámetro omitido equivale a 'T'.
/// </summary>
public sealed class GetValesPaginatedQuery : PagedRequest, IRequest<PagedResult<ValeListadoResponse>>
{
    public string? Codigo_Pla { get; set; }
    public string? Codigo_Val { get; set; }
    public string? Codigo_Est { get; set; }
    public string? Codigo_Usu { get; set; }
    public DateTime? Fecha_Ini { get; set; }
    public DateTime? Fecha_Fin { get; set; }
    public string? Responsable { get; set; }
}
