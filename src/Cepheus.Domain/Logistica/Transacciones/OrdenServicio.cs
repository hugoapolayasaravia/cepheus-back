using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones;

/// <summary>
/// Orden de Servicio (cabecera): contratación de un servicio a un Proveedor, con aprobación por la
/// matriz y procesamiento posterior que actualiza costos de stock y materiales de la Orden de Trabajo.
///
/// Legacy: dbo.MOrdenServicioResI. Es una de las 4 entidades de la Orden de Servicio:
///   OrdenServicio (ResI) + OrdenServicioDetalle (DetI)
///   OrdenServicioSalida (ResS) + OrdenServicioSalidaDetalle (DetS)  <- vale de salida asociado
/// La orden apunta a su vale con ValeSalidaCode (Codigo_Val). El trabajador responsable, la fecha de entrega
/// y los importes en soles viven en el vale, como en el legacy.
///
/// Mapeo de columnas legacy -> propiedades:
///   Codigo_Pla        -> PlantaCode (FK Comunes.Planta)
///   Codigo_NoI        -> Code (correlativo por planta, 6XXXXX)
///   Codigo_tdo        -> ComprobantePagoCode (FK Comunes.ComprobantePago)
///   Numero_Doc        -> NumeroDocumento
///   Codigo_Prv        -> ProveedorCode
///   Moneda            -> MonedaCode (Comunes.Moneda, ISO)
///   Fecha_ent         -> FechaRecepcion (ResS.Fecha_Ent es la misma fecha)
///   Codigo_pgc        -> FormaPagoCode
///   T_Cambio          -> TipoCambio (solo dólares; 0 en soles)
///   Fecha_Pro         -> FechaProceso
///   Codigo_Est        -> Estado (enum, ver EstadoOrdenServicio)
///   Fecha_Emi         -> FechaEmision
///   Igv_Noi / Total_Noi / Monto_Noi -> Igv / Total (suma de líneas) / Monto (importe final)
///   NoGravableSol_dco / RentaSol_dco / FonaviSol_dco / ServicioSol_dco / IgvExtSol_dco
///                     -> NoGravable / Renta / Fonavi / Servicio / IgvExterior
///   Asiento_noi       -> AsientoContable (dato de contabilidad, solo lectura)
///   Usuario           -> CreatedBy (auditoría)
///   Login_apr/Fecha_apr -> AprobadoPor / FechaAprobacion
///   Login_prs         -> ProcesadoPor (la fecha de proceso está en el vale: Fecha_prs de ResS)
///   Codigo_Cop        -> CompradorCode
///   Codigo_Env        -> LugarEnvioCode
///   Codigo_Trm        -> TramiteCode
///   Observacion_Com / Observacion_Co1 -> Observaciones1 / Observaciones2
///   Codigo_Not        -> NotaCompraCode
///   Codigo_Une        -> UnidadNegocioCode
///   Codigo_Val        -> ValeSalidaCode (FK al vale de salida)
///
/// Columnas legacy que NO se migran (pertenecen a la nota de ingreso de compras, no al servicio):
///   Condicion_NoI, Origen_Noi, Codigo_com, Codigo_td1, Codigo_Mot, Numero_Fac (nota de crédito),
///   Emitir_Com, Codigo_MotC, Fecha_Doc.
/// </summary>
public class OrdenServicio : IAuditableEntity
{
    public string PlantaCode { get; set; } = default!;
    public Planta Planta { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string ComprobantePagoCode { get; set; } = default!;
    public ComprobantePago ComprobantePago { get; set; } = default!;

    public string? NumeroDocumento { get; set; }

    public string ProveedorCode { get; set; } = default!;
    public Proveedor Proveedor { get; set; } = default!;

    public string MonedaCode { get; set; } = default!;
    public Moneda Moneda { get; set; } = default!;

    public string FormaPagoCode { get; set; } = default!;
    public FormaPago FormaPago { get; set; } = default!;

    public DateTime FechaRecepcion { get; set; }
    public DateTime FechaEmision { get; set; }
    /// <summary>Solo se usa cuando el comprobante es en dólares (0 en soles).</summary>
    public decimal TipoCambio { get; set; }

    public EstadoOrdenServicio Estado { get; set; } = EstadoOrdenServicio.Pendiente;

    public decimal Igv { get; set; }
    /// <summary>Valor de compra: suma de las líneas.</summary>
    public decimal Total { get; set; }
    /// <summary>Importe final de la orden (tras IGV, renta, fonavi, etc.).</summary>
    public decimal Monto { get; set; }
    public decimal NoGravable { get; set; }
    public decimal Renta { get; set; }
    public decimal Fonavi { get; set; }
    public decimal Servicio { get; set; }
    public decimal IgvExterior { get; set; }

    public string? AsientoContable { get; set; }

    public string? AprobadoPor { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? ProcesadoPor { get; set; }

    public string CompradorCode { get; set; } = default!;
    public Comprador Comprador { get; set; } = default!;

    public string LugarEnvioCode { get; set; } = default!;
    public LugarEnvio LugarEnvio { get; set; } = default!;

    public string TramiteCode { get; set; } = default!;
    public Tramite Tramite { get; set; } = default!;

    public string? NotaCompraCode { get; set; }
    public NotaCompra? NotaCompra { get; set; }

    public string UnidadNegocioCode { get; set; } = default!;
    public UnidadNegocio UnidadNegocio { get; set; } = default!;

    /// <summary>Código del vale de salida asociado (Codigo_Val). Es el mismo número de la orden.</summary>
    public string ValeSalidaCode { get; set; } = default!;
    public OrdenServicioSalida ValeSalida { get; set; } = default!;

    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }

    public ICollection<OrdenServicioDetalle> Detalles { get; set; } = new List<OrdenServicioDetalle>();

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = default!;
}
