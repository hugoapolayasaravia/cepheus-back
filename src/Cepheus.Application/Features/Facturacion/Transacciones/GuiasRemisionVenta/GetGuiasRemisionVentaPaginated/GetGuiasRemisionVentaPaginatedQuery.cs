using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.GetGuiasRemisionVentaPaginated;

/// <summary>
/// Filtros ('T' o vacío = todos): gui (número), ord (OT), pro (programación), cli, obr, pla (planta), est (1 Pendiente,
/// 2 Facturado, 3 Cancelado, 4 Anulado...), mot (motivo), doc (número de documento) y fecha o rango fIni/fFin.
/// Sin ningún filtro se listan las guías de hoy.
/// </summary>
public sealed class GetGuiasRemisionVentaPaginatedQuery
    : PagedRequest, IRequest<PagedResult<GuiaRemisionVentaListadoResponse>>
{
    public string? gui { get; set; }
    public string? ord { get; set; }
    public string? pro { get; set; }
    public string? cli { get; set; }
    public string? obr { get; set; }
    public string? pla { get; set; }
    public int? est { get; set; }
    public int? mot { get; set; }
    public string? doc { get; set; }
    public DateTime? fecha { get; set; }
    public DateTime? fIni { get; set; }
    public DateTime? fFin { get; set; }
}
