using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;

public sealed record PropagacionIncidencia(
    string ArticuloCode,
    string DocumentoCode,
    TipoIncidenciaCierre Tipo,
    string Mensaje);

public sealed record PropagacionResultado(
    IReadOnlyList<RevaluacionAplicada> Aplicadas,
    IReadOnlyList<PropagacionIncidencia> Incidencias)
{
    public static PropagacionResultado Vacio { get; } =
        new(Array.Empty<RevaluacionAplicada>(), Array.Empty<PropagacionIncidencia>());
}

/// <summary>
/// Segunda pasada del cierre: cuando un precio corregido en un documento debe llevarse también a otro documento
/// (la transferencia: el costo final de la salida del origen es el costo del ingreso en el destino).
/// Recibe todo lo que los revaluadores corrigieron en el período y devuelve lo que corrigió a su vez, que el cierre
/// audita igual que el resto. No ejecuta SaveChanges.
/// </summary>
public interface IPropagadorRevaluacion
{
    Task<PropagacionResultado> PropagarAsync(
        string plantaCode,
        string periodCode,
        IReadOnlyList<RevaluacionAplicada> aplicadas,
        CancellationToken cancellationToken);
}
