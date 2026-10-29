using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.GetHojasCortePaginated;

/// <summary>
/// Parámetros literales de ges_sp_bus_hoja_corte:
///   p_anio (año) | p_nrohco (código de 8 dígitos)
///   p_fecini / p_fecfin (rango por fecha de la hoja; cualquiera puede omitirse)
///   p_codcli (5) | p_codobr (3) | p_codven (4) | p_codtec (5)
///   p_codest (estado: 01, 04, 13 — también se acepta el nombre) | p_tiphco (tipo de hoja de corte, 2)
///   p_nomcli / p_nomobr (texto contenido en el nombre de cliente / obra de la hoja).
/// Igual que el SP: si p_nrohco tiene 8 caracteres se busca solo por p_anio + p_nrohco y se ignoran los demás
/// filtros; en caso contrario el año NO filtra y se aplican los demás. Un parámetro vacío o con longitud distinta
/// a la esperada no filtra. Orden por defecto: fecha DESC, código DESC.
/// </summary>
public sealed class GetHojasCortePaginatedQuery
    : PagedRequest, IRequest<PagedResult<HojaCorteListadoResponse>>
{
    public string? p_anio { get; set; }
    public string? p_nrohco { get; set; }
    public DateTime? p_fecini { get; set; }
    public DateTime? p_fecfin { get; set; }
    public string? p_codcli { get; set; }
    public string? p_codobr { get; set; }
    public string? p_codven { get; set; }
    public string? p_codtec { get; set; }
    public string? p_codest { get; set; }
    public string? p_tiphco { get; set; }
    public string? p_nomcli { get; set; }
    public string? p_nomobr { get; set; }
}
