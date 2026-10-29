using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Vale de salida de una Orden de Servicio (cabecera). Es el movimiento de salida que acompaña a la
/// orden: el responsable que recibe el servicio, la fecha de entrega y los importes en SOLES.
///
/// Legacy: dbo.MOrdenServicioResS, PK (Codigo_Pla, Codigo_Val).
///   Codigo_Pla    -> PlantaCode
///   Codigo_Val    -> Code. Se genera con el MISMO número de la Orden de Servicio: aunque el legacy tenía
///                    dos correlativos independientes, Procesar actualiza el vale con Codigo_Val =
///                    Codigo_NoI, o sea que en la práctica eran uno solo. OrdenServicio.ValeSalidaCode
///                    apunta a este Code (Codigo_Val de la tabla MOrdenServicioResI).
///   Fecha_Pro     -> FechaProceso
///   Fecha_Ent     -> FechaEntrega (la misma fecha de recepción de la orden)
///   Codigo_Tra    -> TrabajadorCode (responsable del vale)
///   Neto_Val / Igv_Val / Tot_Val -> Neto / Igv / Total (derivados de las líneas, en soles)
///   Codigo_Est    -> Estado (enum)
///   Usuario       -> CreatedBy (auditoría)
///   Asiento_val   -> AsientoContable
///   Login_apr / Fecha_apr -> AprobadoPor / FechaAprobacion
///   Login_prs / Fecha_prs -> ProcesadoPor / FechaProcesado
/// </summary>
public class OrdenServicioSalida : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    public string Code { get; set; } = default!;

    public DateTime FechaEntrega { get; set; }

    public string TrabajadorCode { get; set; } = default!;
    public Trabajador Trabajador { get; set; } = default!;

    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }

    public EstadoOrdenServicioSalida Estado { get; set; } = EstadoOrdenServicioSalida.Pendiente;

    public string? AsientoContable { get; set; }

    public string? AprobadoPor { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? ProcesadoPor { get; set; }
    public DateTime? FechaProcesado { get; set; }

    public ICollection<OrdenServicioSalidaDetalle> Detalles { get; set; } = new List<OrdenServicioSalidaDetalle>();

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
