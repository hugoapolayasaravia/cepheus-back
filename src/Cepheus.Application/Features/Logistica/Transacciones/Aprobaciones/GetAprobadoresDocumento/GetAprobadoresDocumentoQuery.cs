using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetAprobadoresDocumento;

/// <summary>Quiénes pueden aprobar hoy un documento (legacy: Consulta_Aprobadores_x_Transaccion).</summary>
public sealed record GetAprobadoresDocumentoQuery(string Tipo, string PlantaCode, string Code)
    : IRequest<AprobadoresDocumentoResponse>;

public sealed class AprobadoresDocumentoResponse
{
    public string TipoTransaccion { get; set; } = default!;
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public decimal Monto { get; set; }
    public string MonedaCode { get; set; } = default!;
    public List<AprobadorDocumentoItem> Aprobadores { get; set; } = new();
    /// <summary>Por qué un titular no cubre el documento (importe máximo / acumulado diario o mensual).</summary>
    public List<string> Observaciones { get; set; } = new();
}

public sealed class AprobadorDocumentoItem
{
    public string NivelCode { get; set; } = default!;
    public string Rol { get; set; } = default!;
    public string TrabajadorCode { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string? Login { get; set; }
    public string? Email { get; set; }
}
