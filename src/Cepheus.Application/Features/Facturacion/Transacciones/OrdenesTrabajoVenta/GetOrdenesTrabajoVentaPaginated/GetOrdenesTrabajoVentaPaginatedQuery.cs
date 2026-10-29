using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetOrdenesTrabajoVentaPaginated;

/// <summary>
/// Parámetros literales de ges_ordenTrabajo ('T' o vacío = todos):
///   ord (código de OT, 8) | fecIni (fecha del día) | cli (5) | obr (3) | ven (4) | est (estado: 05, 06, 11, 42)
///   tpr (tipo de producto de alguna línea) | prd (código de producto de alguna línea) | fIni / fFin (rango de fechas)
///   tip (tipo: N, S, A, D, C) | eta (etapa: P, A, C, G) | ot (OT de origen) | opt ('1' agrega las pendientes de aprobar)
///   sis (sistema; sin efecto: era del sistema externo).
///
/// Igual que el SP: con ord solo se busca esa OT; sin ord se aplican los filtros; si no hay ninguno (ni rango de
/// fechas) se listan las OT de la fecha fecIni (hoy si se omite) y se aplica tip. Con opt = '1' se agregan además
/// las OT en etapa Pendiente, Activas y sin aprobar que están fuera de ventana (fecha anterior a fecIni - 5 días,
/// o dentro de [fIni - 5, fFin)), con los mismos filtros.
///
/// vista: T (todas), P (Producción: solo etapa C/G con pase a producción) o C (Créditos: solo OT bloqueadas).
/// Reemplaza al rol del usuario (etapa_mus_ot) del legacy y se cambiará por permisos.
/// Orden por defecto: fecha DESC, código DESC.
/// </summary>
public sealed class GetOrdenesTrabajoVentaPaginatedQuery
    : PagedRequest, IRequest<PagedResult<OrdenTrabajoVentaListadoResponse>>
{
    public string? ord { get; set; }
    public DateTime? fecIni { get; set; }
    public string? cli { get; set; }
    public string? obr { get; set; }
    public string? ven { get; set; }
    public string? est { get; set; }
    public string? tpr { get; set; }
    public string? prd { get; set; }
    public DateTime? fIni { get; set; }
    public DateTime? fFin { get; set; }
    public string? tip { get; set; }
    public string? eta { get; set; }
    public string? ot { get; set; }
    public string? opt { get; set; }
    public string? sis { get; set; }
    public string? vista { get; set; }
}
