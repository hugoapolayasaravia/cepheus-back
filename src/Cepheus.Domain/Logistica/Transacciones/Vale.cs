using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Maestros;
using Cepheus.Domain.Mantenimiento.Transacciones;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Vale de Salida (cabecera): entrega de artículos del almacén a un subcentro de costo / orden de trabajo.
///
/// Legacy: dbo.MValesRes (SQL Server). Mapeo de columnas legacy -> propiedades:
///   Codigo_Pla    -> PlantaCode (almacén que entrega; FK Comunes.Planta)
///   Codigo_Val    -> Code (correlativo por planta, 9XXXXXX, char(7))
///   Codigo_tva    -> TipoValeCode (FK Logistica.TipoVale)
///   Fecha_Pro     -> CreatedAt (auditoría)
///   Fecha_Ent     -> FechaEntrega
///   Codigo_Scc    -> SubCentroCostoCode (FK Logistica.SubCentroCosto)
///   Codigo_Sce    -> SubCentroEjecutorCode (FK Mantenimiento.SubCentroEjecutor, opcional)
///   Codigo_Tra    -> TrabajadorCode (FK Rrhh.Trabajador; responsable)
///   Codigo_Otr    -> OrdenTrabajoCode (FK compuesta Planta + OT, opcional)
///   Codigo_Une    -> UnidadNegocioCode (FK Logistica.UnidadNegocio)
///   Neto_Val      -> Neto
///   Igv_Val       -> Igv
///   Tot_Val       -> Total
///   Codigo_Est    -> Estado (enum, ver EstadoVale)
///   Usuario       -> CreatedBy (auditoría)
///   Cod_Planta    -> PlantaAfectadaCode (planta beneficiada; FK Comunes.Planta)
///   Asiento_val   -> AsientoContable (dato de contabilidad, se mantiene)
///   Login_apr / Fecha_apr -> AprobadoPor / FechaAprobacion
///   Login_prs / Fecha_prs -> ProcesadoPor / FechaProcesado
///   Login_anu / Fecha_anu -> AnuladoPor / FechaAnulacion
///   Prepara_val   -> Preparado ('S' / null  =>  true / false)
/// </summary>
public class Vale : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string TipoValeCode { get; set; } = default!;
    public TipoVale TipoVale { get; set; } = default!;

    public DateTime FechaEntrega { get; set; }

    public string SubCentroCostoCode { get; set; } = default!;
    public SubCentroCosto SubCentroCosto { get; set; } = default!;

    public string? SubCentroEjecutorCode { get; set; }
    public SubCentroEjecutor? SubCentroEjecutor { get; set; }

    public string TrabajadorCode { get; set; } = default!;
    public Trabajador Trabajador { get; set; } = default!;

    public string? OrdenTrabajoCode { get; set; }
    public OrdenTrabajo? OrdenTrabajo { get; set; }

    public string UnidadNegocioCode { get; set; } = default!;
    public UnidadNegocio UnidadNegocio { get; set; } = default!;

    public string PlantaAfectadaCode { get; set; } = default!;
    public Planta PlantaAfectada { get; set; } = default!;

    public decimal Neto { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }

    public EstadoVale Estado { get; set; } = EstadoVale.Pendiente;

    public string? AsientoContable { get; set; }

    public string? AprobadoPor { get; set; }
    public DateTime? FechaAprobacion { get; set; }

    public string? ProcesadoPor { get; set; }
    public DateTime? FechaProcesado { get; set; }

    public string? AnuladoPor { get; set; }
    public DateTime? FechaAnulacion { get; set; }

    public bool Preparado { get; set; }

    public ICollection<ValeDetalle> Detalles { get; set; } = new List<ValeDetalle>();

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
