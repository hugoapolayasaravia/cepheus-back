namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesOtros.Common;

/// <summary>Datos de la cabecera de una programación de Otros (la OT se define al crear y no cambia).</summary>
public class ProgramacionOtrosFields
{
    /// <summary>Fecha de programación (solo fecha). Hasta 7 días antes de hoy.</summary>
    public DateTime ProgramDate { get; set; }

    /// <summary>Hora "HH:mm" (hora 01-23, minuto 00-59).</summary>
    public string ProgramTime { get; set; } = default!;

    /// <summary>Planta desde la que se despacha.</summary>
    public string PlantCode { get; set; } = default!;

    public string? TransportObservations { get; set; }
}

public sealed class ProgramacionOtrosDetalleRequest
{
    /// <summary>Item (Clave) de la línea de la OT que se programa (negocio distinto de Concreto).</summary>
    public int Item { get; set; }

    /// <summary>Cantidad a programar (mayor a cero y dentro de lo aprobado aún no programado).</summary>
    public decimal Quantity { get; set; }
}
