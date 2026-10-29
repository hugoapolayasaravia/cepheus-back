using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjustesInventarioPaginated;

/// <summary>
/// Parámetros literales de Logi_sp_Listado_MAjustes:
///   Codigo_Pla (obligatorio) | Codigo_Aju ('T' = todos, o un código)
///   Codigo_Est ('T' o estado: 01, 12, 13, 14, 04 — también se acepta el nombre)
///   Codigo_Usu ('T' o texto contenido en el usuario que lo registró)
///   Fecha_Ini / Fecha_Fin (filtran por fecha de proceso; obligatorias cuando Codigo_Aju = 'T')
///   Tipo_Aju ('T', 'I' sobrante o 'S' faltante: ajustes que tienen alguna línea de ese tipo).
/// El SP recibe Tipo_Aju pero no lo usa; acá se aplica como filtro por línea.
/// Los filtros de fechas, estado y usuario solo aplican con Codigo_Aju = 'T', como el SP.
/// Un parámetro omitido equivale a 'T'.
/// </summary>
public sealed class GetAjustesInventarioPaginatedQuery
    : PagedRequest, IRequest<PagedResult<AjusteInventarioListadoResponse>>
{
    public string? Codigo_Pla { get; set; }
    public string? Codigo_Aju { get; set; }
    public string? Codigo_Est { get; set; }
    public string? Codigo_Usu { get; set; }
    public DateTime? Fecha_Ini { get; set; }
    public DateTime? Fecha_Fin { get; set; }
    public string? Tipo_Aju { get; set; }
}
