using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetPendientes;

/// <summary>
/// Listado de pendientes de un tipo (SC, OC, OS, VS, VC). Todos = false: solo los que el usuario puede aprobar.
/// </summary>
public sealed class GetPendientesQuery : PagedRequest, IRequest<PagedResult<PendienteResponse>>
{
    public string Tipo { get; set; } = default!;
    public bool Todos { get; set; }
    public string? Search { get; set; }
}

public sealed class PendienteResponse
{
    public string TipoTransaccion { get; set; } = default!;
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string UnidadNegocioCode { get; set; } = default!;
    public string MonedaCode { get; set; } = default!;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    /// <summary>Trabajador (pedido / vale) o proveedor (OC / OS).</summary>
    public string? Contraparte { get; set; }
    public string? CreatedBy { get; set; }
}
