using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.GetProgramacionesOtrosPaginated;

/// <summary>
/// Filtros ('T' o vacío = todos): pro (código), ord (OT), cli, obr, ven, pla (planta), est (01 Pendiente, 04 Anulada,
/// 11 Cerrada) y fecha_pro o rango fIni/fFin. Sin ningún filtro se listan las programaciones de hoy.
/// </summary>
public sealed class GetProgramacionesOtrosPaginatedQuery
    : PagedRequest, IRequest<PagedResult<ProgramacionOtrosListadoResponse>>
{
    public string? pro { get; set; }
    public string? ord { get; set; }
    public string? cli { get; set; }
    public string? obr { get; set; }
    public string? ven { get; set; }
    public string? pla { get; set; }
    public string? est { get; set; }
    public DateTime? fecha_pro { get; set; }
    public DateTime? fIni { get; set; }
    public DateTime? fFin { get; set; }
}
