using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.GetOrdenesServicioSalidaPaginated;

/// <summary>
/// Listado de vales de salida de Orden de Servicio. El legacy no tenía un listado propio (el vale se consultaba
/// desde la orden), así que se siguen las mismas convenciones del listado de la orden:
///   Codigo_Pla (obligatorio) | Codigo_Val ('T' o código) | Codigo_Tra ('T' o trabajador)
///   Codigo_Est ('T' o 01, 13, 04 / Pendiente, Procesado, Anulado)
///   Fecha_Ini / Fecha_Fin (obligatorias con Codigo_Val = 'T'; filtran por fecha de proceso).
/// </summary>
public sealed class GetOrdenesServicioSalidaPaginatedQuery
    : PagedRequest, IRequest<PagedResult<OrdenServicioSalidaListadoResponse>>
{
    public string? Codigo_Pla { get; set; }
    public string? Codigo_Val { get; set; }
    public string? Codigo_Tra { get; set; }
    public string? Codigo_Est { get; set; }
    public DateTime? Fecha_Ini { get; set; }
    public DateTime? Fecha_Fin { get; set; }
}
