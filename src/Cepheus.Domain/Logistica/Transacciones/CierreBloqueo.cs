namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Fila de bloqueo por planta para serializar el cierre mensual (y su reapertura). El proceso actualiza esta fila al
/// empezar su transacción: cualquier otro cierre de la misma planta espera a que termine. Usa solo un UPDATE normal,
/// por lo que funciona igual en cualquier motor de base de datos (no depende de sp_getapplock ni de advisory locks).
/// </summary>
public class CierreBloqueo
{
    public string PlantaCode { get; set; } = default!;

    /// <summary>Última vez que un proceso tomó el bloqueo (UTC).</summary>
    public DateTime UltimoUso { get; set; }
}
