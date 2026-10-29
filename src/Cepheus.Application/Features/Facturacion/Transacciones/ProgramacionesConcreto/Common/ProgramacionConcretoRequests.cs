namespace Cepheus.Application.Features.Facturacion.Transacciones.ProgramacionesConcreto.Common;

/// <summary>Datos de la cabecera de una programación de Concreto (la OT se define al crear y no cambia).</summary>
public class ProgramacionConcretoFields
{
    /// <summary>Fecha de programación (solo fecha). Hasta 7 días antes de hoy.</summary>
    public DateTime ProgramDate { get; set; }

    /// <summary>Hora "HH:mm" (hora 01-23, minuto 00-59).</summary>
    public string ProgramTime { get; set; } = default!;

    public string PlantCode { get; set; } = default!;
    public string ElementoCode { get; set; } = default!;

    public string? TransportObservations { get; set; }
    public bool RequiresLabService { get; set; }
}

/// <summary>Datos de una línea programada.</summary>
public class ProgramacionConcretoDetalleFields
{
    /// <summary>Cantidad a programar (mayor a cero y dentro de lo aprobado aún no programado).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Solo bombeo (tipo 04): código de la bomba. Obligatorio en esa línea.</summary>
    public string? PumpCode { get; set; }

    /// <summary>Solo bombeo (tipo 04): apoyo 0 a 9. Obligatorio en esa línea.</summary>
    public byte? Support { get; set; }
}

public sealed class ProgramacionConcretoDetalleRequest : ProgramacionConcretoDetalleFields
{
    /// <summary>Item (Clave) de la línea de la OT que se programa.</summary>
    public int Item { get; set; }
}
